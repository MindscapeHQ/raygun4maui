using Mindscape.Raygun4Net;
using Raygun4Maui.MauiRUM.EventTrackers;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM;

public class RaygunRum
{
    public bool Enabled { get; private set; }

    private RaygunViewTracker _viewTracker;
    private RaygunSessionTracker _sessionTracker;
    private RaygunNetworkTracker _raygunNetworkTracker;
    
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
        
        System.Diagnostics.Debug.WriteLine("SETTING UP VIEW TRACKER");
        
        
        _viewTracker = new RaygunViewTracker();
        _sessionTracker = new RaygunSessionTracker();
        _raygunNetworkTracker = new RaygunNetworkTracker();
        
        _requestHandler = new RaygunWebRequestHandler(_mauiSettings.RaygunSettings.ApiKey, _mauiSettings.RumApiEndpoint, 30_0000);

        _viewTracker.ViewLoaded += OnViewLoaded;
        _viewTracker.Init(settings);
        
        _sessionTracker.SessionStarted += OnSendSessionStartedEvent;
        _sessionTracker.SessionChanged += OnSendSessionChangedEvent;
        _sessionTracker.Init(user);
    }
    
    public void UpdateUser(RaygunIdentifierMessage user)
    {
        _sessionTracker.CurrentUser = user;
    }
    
    private void OnViewLoaded(object sender, RaygunTimingEventArgs args)
    {
        System.Diagnostics.Debug.WriteLine("TIMINGS HAPPENED!!!!");
        _sessionTracker.EnsureSessionStarted();

        SendTimingEvent(args.Type, args.Key, args.Milliseconds);

        _sessionTracker.UpdateLastSeenTime();
    }
    
    public void SendTimingEvent(RaygunRumEventTimingType timingType, string name, long duration)
    {
        if (!Enabled)
        {
            // RaygunLogger.Info("Cannot send RUM event - RUM has not been enabled");
            return;
        }

        var message = BuildTimingEventMessage(timingType, name, duration);

        System.Diagnostics.Debug.WriteLine(RaygunSerializer.Serialize(message));
        SendEvent(message);
    }
    
    private RaygunRumMessage BuildTimingEventMessage(RaygunRumEventTimingType timingType, string name, long duration)
    {
        var data = new RaygunRumTimingData[1]
        {
            new RaygunRumTimingData
            {
                Name = name,
                Timing = new RaygunRumTimingInfo
                {
                    Type     = TimingTypeToString(timingType),
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
                    SessionId = _sessionTracker.SessionId,
                    Timestamp = DateTime.UtcNow,
                    Type      = EventTypeToString(RaygunRumEventType.Timing),
                    User      = _sessionTracker.CurrentUser,
                    Version   = _mauiSettings.RaygunSettings.ApplicationVersion ?? UnknownValue,
                    Os        = NativeDeviceInfo.Platform(), // TODO: Investigate this
                    OsVersion = DeviceInfo.Current.VersionString,
                    Platform  = NativeDeviceInfo.Platform(),
                    Data      = RaygunSerializer.Serialize(data)
                }
            }
        };

        return message;
    }
    
    private void OnSendSessionStartedEvent(object sender, RaygunSessionEventArgs args)
    {
        SendSessionEvent(RaygunRumEventType.SessionStart, args.SessionId, args.User);
    }

    private void OnSendSessionChangedEvent(object sender, RaygunSessionChangedEventArgs args)
    {
        SendSessionEvent(RaygunRumEventType.SessionEnd,   args.OldSessionId, args.OldUser);
        SendSessionEvent(RaygunRumEventType.SessionStart, args.NewSessionId, args.NewUser);
    }
    
    private void SendSessionEvent(RaygunRumEventType eventType, string sessionId, RaygunIdentifierMessage user)
    {
        if (!Enabled)
        {
            // RaygunLogger.Info("Cannot send RUM event - RUM has not been enabled");
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

        // TODO: Need to implement network tracker
        // if (timingType == RaygunRumEventTimingType.NetworkCall && _networkTracker.ShouldIgnore(name))
        // {
        //     return;
        // }

        SendTimingEvent(timingType, name, duration);
    }

    private RaygunRumMessage BuildSessionEventMessage(RaygunRumEventType eventType, string sessionId, RaygunIdentifierMessage user)
    {
        var message = new RaygunRumMessage
        {
            EventData = new RaygunRumEventInfo[1]
            {
                new RaygunRumEventInfo
                {
                    SessionId = sessionId,
                    Timestamp = DateTime.UtcNow,
                    Type      = EventTypeToString(eventType),
                    User      = user,
                    Version   = _mauiSettings.RaygunSettings.ApplicationVersion ?? UnknownValue,
                    Os        = NativeDeviceInfo.Platform(), // TODO: Investigate this
                    OsVersion = DeviceInfo.Current.VersionString,
                    Platform  = NativeDeviceInfo.Platform(),
                }
            }
        };

        return message;
    }
    
    private void SendEvent(RaygunRumMessage message)
    {
        string payload = RaygunSerializer.Serialize(message);

        if (_requestHandler.IsOnline())
        {
            // RaygunLogger.Verbose("Sending Payload --------------");
            // RaygunLogger.Verbose(payload);
            // RaygunLogger.Verbose("------------------------------");

            Task.Run(async () =>
                {
                    var responseStatusCode = await _requestHandler.PostAsync(payload);
                    // RaygunLogger.LogResponseStatusCode(responseStatusCode);
                    System.Diagnostics.Debug.WriteLine(responseStatusCode);
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        // RaygunLogger.Error("Fault occurred when sending RUM event");
                        System.Diagnostics.Debug.WriteLine("Fault occurred when sending RUM event");
                    }

                    // Consume all errors as we dont want them being sent.
                    t.Exception?.Handle((e) =>
                    {
                        // RaygunLogger.Error("Error occurred while sending RUM event: " + e.Message);
                        System.Diagnostics.Debug.WriteLine("Error occurred while sending RUM event: " + e.Message);
                        return true; // Handled
                    });
                });
        }
        else
        {
            // RaygunLogger.Debug($"[RaygunRUM] Offline - not sending RUM event");
            System.Diagnostics.Debug.WriteLine($"[RaygunRUM] Offline - not sending RUM event");
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