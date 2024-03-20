using System.Diagnostics;
using System.Text.RegularExpressions;
using Mindscape.Raygun4Net;
using Raygun4Maui.MauiRUM.EventTrackers;
#if ANDROID
using Raygun4Maui.MauiRUM.EventTrackers.Android;
#endif
using Raygun4Maui.MauiRUM.EventTrackers.Apple;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM;

public static class RaygunRum
{
    public static bool Enabled { get; private set; } = false;

    private static IRaygunWebRequestHandler _requestHandler;

    private const string UnknownValue = "Unknown";

    private static Raygun4MauiSettings _mauiSettings;

    public static void Enable(Raygun4MauiSettings settings, RaygunIdentifierMessage user)
    {
        if (Enabled) return;

        Enabled = true;

        // DI Container for this should be set up by this point
        _mauiSettings = settings;

        _requestHandler = new RaygunWebRequestHandler(
            _mauiSettings?.RaygunSettings.ApiKey,
            _mauiSettings?.RumApiEndpoint, 300_000
        );

        RaygunViewTracker.ViewLoaded += OnViewLoaded;
        RaygunViewTracker.Init(settings);


        RaygunSessionTracker.SessionStarted += OnSendSessionStartedEvent;
        RaygunSessionTracker.SessionChanged += OnSendSessionChangedEvent;
        RaygunSessionTracker.Init(user); // Find a nicer way to inject user

        RaygunNetworkTracker.NetworkRequestCompleted += OnNetworkRequestCompletedEvent;
        RaygunNetworkTracker.Init(settings);
    }

    public static void UpdateUser(RaygunIdentifierMessage user)
    {
        RaygunSessionTracker.CurrentUser = user;
    }

    private static void OnViewLoaded(RaygunTimingEventArgs args)
    {
        RaygunSessionTracker.EnsureSessionStarted();

        SendTimingEvent(args.Type, args.Key, args.Milliseconds);

        RaygunSessionTracker.UpdateLastSeenTime();
    }

    private static void OnNetworkRequestCompletedEvent(RaygunTimingEventArgs args)
    {
        RaygunSessionTracker.EnsureSessionStarted();

        SendTimingEvent(args.Type, args.Key, args.Milliseconds);

        RaygunSessionTracker.UpdateLastSeenTime();
    }

    public static void SendTimingEvent(RaygunRumEventTimingType timingType, string name, long duration)
    {
        if (!Enabled)
        {
            return;
        }

        var message = BuildTimingEventMessage(timingType, name, duration);

        SendEvent(message);
    }

    private static RaygunRumMessage BuildTimingEventMessage(RaygunRumEventTimingType timingType, string name,
        long duration)
    {
        var data = new RaygunRumTimingData[1]
        {
            new RaygunRumTimingData
            {
                Name = name,
                Timing = new RaygunRumTimingInfo
                {
                    Type = TimingTypeToString(timingType),
                    Duration = duration
                }
            }
        };

        var message = new RaygunRumMessage
        {
            EventData = new RaygunRumEventInfo[1]
            {
                new RaygunRumEventInfo
                {
                    sessionId = RaygunSessionTracker.SessionId,
                    timestamp = DateTime.UtcNow,
                    type = EventTypeToString(RaygunRumEventType.Timing),
                    user = RaygunSessionTracker.CurrentUser, // Is updated by the MauiUserProvider implementation
                    version = GetAppVersion(),
                    os = NativeDeviceInfo.Platform(),
                    osVersion = NativeDeviceInfo.GetOsVersion(prefix: false),
                    platform = DeviceInfo
                        .Model, // Xamarin uses device model, e.g. iPhone 15, Motherboard Version (windows)
                    data = RaygunSerializer.Serialize(data)
                }
            }
        };


        return message;
    }

    private static String GetAppVersion()
    {
        if (!string.IsNullOrEmpty(_mauiSettings.RaygunSettings.ApplicationVersion))
        {
            return _mauiSettings.RaygunSettings.ApplicationVersion;
        }

        var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
        return entryAssembly?.GetName().Version?.ToString() ?? UnknownValue;
    }

    private static void OnSendSessionStartedEvent(RaygunSessionEventArgs args)
    {
        SendSessionEvent(RaygunRumEventType.SessionStart, args.SessionId, args.User);
    }

    private static void OnSendSessionChangedEvent(RaygunSessionChangedEventArgs args)
    {
        SendSessionEvent(RaygunRumEventType.SessionEnd, args.OldSessionId, args.OldUser);
        SendSessionEvent(RaygunRumEventType.SessionStart, args.NewSessionId, args.NewUser);
    }

    private static void SendSessionEvent(RaygunRumEventType eventType, string sessionId, RaygunIdentifierMessage user)
    {
        if (!Enabled)
        {
            return;
        }

        var message = BuildSessionEventMessage(eventType, sessionId, user);

        SendEvent(message);
    }

    public static void SendCustomTimingEvent(RaygunRumEventTimingType timingType, string name, long duration)
    {
        if (!Enabled)
        {
            return;
        }


        if (timingType == RaygunRumEventTimingType.ViewLoaded && RaygunViewTracker.ShouldIgnore(name))
        {
            return;
        }

        if (timingType == RaygunRumEventTimingType.NetworkCall && RaygunNetworkTracker.ShouldIgnore(name)
           )
        {
            return;
        }

        SendTimingEvent(timingType, name, duration);
    }

    private static RaygunRumMessage BuildSessionEventMessage(RaygunRumEventType eventType, string sessionId,
        RaygunIdentifierMessage user)
    {
        var message = new RaygunRumMessage
        {
            EventData = new RaygunRumEventInfo[1]
            {
                new RaygunRumEventInfo
                {
                    sessionId = sessionId,
                    timestamp = DateTime.UtcNow,
                    type = EventTypeToString(eventType),
                    user = user,
                    version = GetAppVersion(),
                    os = NativeDeviceInfo.Platform(),
                    osVersion = NativeDeviceInfo.GetOsVersion(prefix: false),
                    platform = DeviceInfo.Model,
                }
            }
        };
        return message;
    }

    private static async void SendEvent(RaygunRumMessage message)
    {
        var payload = RaygunSerializer.Serialize(message);

        var isOnline = await _requestHandler.IsOnline();
        if (isOnline)
        {
            await _requestHandler.PostAsync(payload);
        }
    }

    private static string EventTypeToString(RaygunRumEventType eventType)
    {
        return eventType switch
        {
            RaygunRumEventType.SessionStart => "session_start",
            RaygunRumEventType.Timing => "mobile_event_timing",
            RaygunRumEventType.SessionEnd => "session_end",
            _ => ""
        };
    }

    private static string TimingTypeToString(RaygunRumEventTimingType timingType)
    {
        return timingType switch
        {
            RaygunRumEventTimingType.ViewLoaded => "p",
            RaygunRumEventTimingType.NetworkCall => "n",
            RaygunRumEventTimingType.CustomTiming => "t",
            _ => throw new ArgumentOutOfRangeException(nameof(timingType), timingType, null)
        };
    }
}