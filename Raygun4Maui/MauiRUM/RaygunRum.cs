using System.Diagnostics;
using Mindscape.Raygun4Net;
using Raygun4Maui.MauiRUM.EventTrackers;
using Raygun4Maui.MauiRUM.EventTrackers.Apple;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM;

public class RaygunRum
{
    public bool Enabled { get; private set; }

    private RaygunViewTracker _viewTracker;
    private RaygunSessionTracker _sessionTracker;
    private RaygunNetworkTracker _networkTracker;

    private IRaygunWebRequestHandler _requestHandler;


    private const string UnknownValue = "Unknown";

    private Raygun4MauiSettings _mauiSettings;
    
    public RaygunRum()
    {
    }

    public void Enable(Raygun4MauiSettings settings, RaygunIdentifierMessage user)
    {
        if (Enabled) return;

        Enabled = true;

        _mauiSettings = settings;

        _viewTracker = new RaygunViewTracker();
        _sessionTracker = new RaygunSessionTracker();
        _networkTracker = new RaygunNetworkTracker();

        // DiagnosticListener.AllListeners.Subscribe(_networkTracker);

        _requestHandler =
            new RaygunWebRequestHandler(_mauiSettings.RaygunSettings.ApiKey, _mauiSettings.RumApiEndpoint, 30_0000);

        _viewTracker.ViewLoaded += OnViewLoaded;
        _viewTracker.Init(settings);
        

        _sessionTracker.SessionStarted += OnSendSessionStartedEvent;
        _sessionTracker.SessionChanged += OnSendSessionChangedEvent;
        _sessionTracker.Init(user);

        // _networkTracker.NetworkRequestCompleted += OnNetworkRequestCompletedEvent;
        // _networkTracker.Init(settings);
    }

    public void UpdateUser(RaygunIdentifierMessage user)
    {
        _sessionTracker.CurrentUser = user;
    }

    private void OnViewLoaded(RaygunTimingEventArgs args)
    {
        _sessionTracker.EnsureSessionStarted();

        SendTimingEvent(args.Type, args.Key, args.Milliseconds);

        _sessionTracker.UpdateLastSeenTime();
    }

    private void OnNetworkRequestCompletedEvent(RaygunTimingEventArgs args)
    {
        _sessionTracker.EnsureSessionStarted();

        SendTimingEvent(args.Type, args.Key, args.Milliseconds);

        _sessionTracker.UpdateLastSeenTime();
    }

    public void SendTimingEvent(RaygunRumEventTimingType timingType, string name, long duration)
    {
        if (!Enabled)
        {
            return;
        }

        var message = BuildTimingEventMessage(timingType, name, duration);

        SendEvent(message);
    }

    private RaygunRumMessage BuildTimingEventMessage(RaygunRumEventTimingType timingType, string name, long duration)
    {
        var data = new RaygunRumTimingData[1]
        {
            new RaygunRumTimingData
            {
                name = name,
                timing = new RaygunRumTimingInfo
                {
                    type = TimingTypeToString(timingType),
                    duration = duration
                }
            }
        };

        var message = new RaygunRumMessage
        {
            EventData = new RaygunRumEventInfo[1]
            {
                new RaygunRumEventInfo
                {
                    sessionId = _sessionTracker.SessionId,
                    timestamp = DateTime.UtcNow,
                    type = EventTypeToString(RaygunRumEventType.Timing),
                    user = _sessionTracker.CurrentUser,
                    version = _mauiSettings.RaygunSettings.ApplicationVersion ?? UnknownValue,
                    os = NativeDeviceInfo
                        .Platform(), // Cannot get specific Windows version, e.g. Windows 10 vs 11 so we use general platform
#if WINDOWS
                    osVersion = GetWindowsVersion(DeviceInfo.Current.VersionString),
#else
                    osVersion = DeviceInfo.Current.VersionString,
#endif
                    platform = DeviceInfo
                        .Model, // Xamarin uses device model, e.g. iPhone 15, Motherboard Version (windows)
                    data = RaygunSerializer.Serialize(data)
                }
            }
        };


        return message;
    }

    private void OnSendSessionStartedEvent(RaygunSessionEventArgs args)
    {
        SendSessionEvent(RaygunRumEventType.SessionStart, args.SessionId, args.User);
    }

    private void OnSendSessionChangedEvent(RaygunSessionChangedEventArgs args)
    {
        SendSessionEvent(RaygunRumEventType.SessionEnd, args.OldSessionId, args.OldUser);
        SendSessionEvent(RaygunRumEventType.SessionStart, args.NewSessionId, args.NewUser);
    }

    private void SendSessionEvent(RaygunRumEventType eventType, string sessionId, RaygunIdentifierMessage user)
    {
        if (!Enabled)
        {
            return;
        }

        var message = BuildSessionEventMessage(eventType, sessionId, user);

        SendEvent(message);
    }

    public void SendCustomTimingEvent(RaygunRumEventTimingType timingType, string name, long duration)
    {
        if (!Enabled) return;


        if (timingType == RaygunRumEventTimingType.ViewLoaded && _viewTracker.ShouldIgnore(name))
        {
            return;
        }

        if (timingType == RaygunRumEventTimingType.NetworkCall && _networkTracker.ShouldIgnore(name))
        {
            return;
        }

        SendTimingEvent(timingType, name, duration);
    }

    private RaygunRumMessage BuildSessionEventMessage(RaygunRumEventType eventType, string sessionId,
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
                    version = _mauiSettings.RaygunSettings.ApplicationVersion ?? UnknownValue,
                    os = NativeDeviceInfo.Platform(),
#if WINDOWS
                    osVersion = GetWindowsVersion(DeviceInfo.Current.VersionString),
#else
                    osVersion = DeviceInfo.Current.VersionString,
#endif
                    platform = DeviceInfo.Model,
                }
            }
        };

        return message;
    }

    private async void SendEvent(RaygunRumMessage message)
    {
        var payload = RaygunSerializer.Serialize(message);

        var isOnline = await _requestHandler.IsOnline();
        if (isOnline)
        {
            // RaygunLogger.Verbose("Sending Payload --------------");
            // RaygunLogger.Verbose(payload);
            // RaygunLogger.Verbose("------------------------------");

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

    private static string GetWindowsVersion(string buildNumber)
    {
        // Extracting the build number (assuming it's in the format "10.0.xxxxx.xxxx")
        var parts = buildNumber.Split('.');
        if (parts.Length < 3)
        {
            return buildNumber;
        }

        if (!int.TryParse(parts[2], out var buildPart))
        {
            return buildNumber;
        }

        // Comparing the build number to determine the Windows version (Windows 10 will never be higher than 22000)
        return buildPart < 22000 ? "10" : "11";
    }
}