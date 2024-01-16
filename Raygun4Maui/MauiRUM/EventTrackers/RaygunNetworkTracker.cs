using System.Diagnostics;
using System.Reflection;
using Microsoft.Maui.Platform;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTrackers.Apple;

#if ANDROID
using Android.Content;
using Com.Raygun.Networkmonitorlibrary;
using Raygun4Maui.MauiRUM.EventTrackers.Android;
#endif

using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunNetworkTracker

    // : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
{
    private const string NETWORK_TIMING_INTENT_ACTION = "com.raygun.networkmonitorlibrary.NetworkRequestTiming";

    // private readonly Dictionary<string, DateTime>
    //     _requestStartTimes =
    //         new Dictionary<string, DateTime>(); // TODO: Possible memory leak if there is no RequestEnd event
    //
    private Raygun4MauiSettings _settings;

    private static RaygunNetworkTracker _instance;

    public static RaygunNetworkTracker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RaygunNetworkTracker();
            }

            return _instance;
        }
    }

    //
    public event Action<RaygunTimingEventArgs> NetworkRequestCompleted;

#if ANDROID
    private RaygunAndroidNativeNetworkTracker androidTracker;
    private RaygunAndroidNetworkReceiver androidReciever;
#endif

#if IOS || MACCATALYST
    private RaygunAppleNativeNetworkObserver _appleNetworkObserver;
#endif

    public RaygunNetworkTracker()
    {
        // RaygunAppEventPublisher.Instance.AppStarted += OnAppStarted;
    }

    private void OnAppStarted(AppStarted obj)
    {
#if ANDROID
        var activity = Platform.CurrentActivity;
        
        Console.WriteLine($"Setting up Broadcast Receiver, Activity: {activity != null}");
        
        androidReciever = new RaygunAndroidNetworkReceiver();
        IntentFilter filter = new IntentFilter(NETWORK_TIMING_INTENT_ACTION);

        // activity?.RegisterReceiver(androidReciever, filter);
        
        activity?.ApplicationContext?.RegisterReceiver(androidReciever, filter);
        // LocalBroadcastManager.GetInstance(activity?.ApplicationContext)?.RegisterReceiver(androidReciever, new IntentFilter(NETWORK_TIMING_INTENT_ACTION));
        try
        {
            RaygunNetworkMonitor.SetContext(activity?.ApplicationContext);
            Com.Raygun.Networkmonitorlibrary.Test.RaygunNetworkRequestTest.PerformHttpGetRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to pass the context to the network monitor due to: {e.Message}");
        }        
        Console.WriteLine("Receiver registered");
#endif
    }

    //
    //
    // private const string Request = "Request";
    // private const string RequestStart = "System.Net.Http.HttpRequestOut.Start";
    // private const string RequestEnd = "System.Net.Http.HttpRequestOut.Stop";
    // private const string RaygunId = "X-Raygun-Request-ID";
    //
    public void Init(Raygun4MauiSettings settings)
    {
        Trace.WriteLine("Initialized Network Tracker");
        _settings = settings;

        RaygunAppEventPublisher.Instance.NetworkRequestFinished += OnNetworkRequestFinishedEvent;

        if (settings.RumFeatureFlags.HasFlag(RumFeatures.Network))
        {
#if ANDROID
            androidTracker = new RaygunAndroidNativeNetworkTracker();
            androidTracker.Enable();
#endif

#if IOS || MACCATALYST
            _appleNetworkObserver = new RaygunAppleNativeNetworkObserver();
            _appleNetworkObserver.Register();
#endif
        }
    }

    //
    // public void OnNext(DiagnosticListener listener)
    // {
    //     Console.WriteLine($"Subscribing to handler {listener.Name}");
    //     if (listener.Name == "HttpHandlerDiagnosticListener")
    //     {
    //         listener.Subscribe(this);
    //     }
    // }
    //
    // public void OnNext(KeyValuePair<string, object> value)
    // {
    //     Console.WriteLine($"New HTTP Event: {value.Key} \n {value.Value}");
    //     
    //
    //     switch (value.Key)
    //     {
    //         case RequestStart:
    //         {
    //             if (value.Value?.GetType().GetProperty("Request")?.GetValue(value.Value) is not HttpRequestMessage
    //                 request)
    //             {
    //                 return;
    //             }
    //
    //             // TODO: Make this a function to check proxy and raygun
    //             if (request.RequestUri?.Host.Contains("api.raygun.com") ??
    //                 true) // Raygun4Net can set proxy, also do a test case for the RaygunId
    //             {
    //                 return;
    //             }
    //
    //             // Check if X-Request-ID already exists
    //             if (!request.Headers.Contains(RaygunId)) // Constant (Raygun)
    //             {
    //                 var trackingId = Guid.NewGuid().ToString();
    //                 request.Headers.Add(RaygunId, trackingId);
    //                 _requestStartTimes[trackingId] = DateTime.UtcNow;
    //             }
    //             else
    //             {
    //                 // Use existing X-Request-ID
    //                 var existingTrackingId = request.Headers.GetValues(RaygunId).FirstOrDefault();
    //                 if (existingTrackingId != null)
    //                 {
    //                     _requestStartTimes[existingTrackingId] =
    //                         DateTime.UtcNow; // TODO: Inject DateTime as a service to unit test
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case RequestEnd:
    //         {
    //             if (value.Value?.GetType().GetProperty("Request")?.GetValue(value.Value) is not HttpRequestMessage
    //                 request)
    //             {
    //                 return;
    //             }
    //
    //             // TODO: Fix ordering
    //             if (request.RequestUri?.Host.Contains("api.raygun.com") ?? true)
    //             {
    //                 return;
    //             }
    //
    //             if (!request.Headers.TryGetValues(RaygunId, out var values))
    //             {
    //                 return;
    //             }
    //
    //             var trackingId = values.FirstOrDefault();
    //
    //             if (trackingId == null) return;
    //             if (!_requestStartTimes.TryGetValue(trackingId, out var startTime))
    //             {
    //                 return;
    //             }
    //
    //             var endTime = DateTime.UtcNow;
    //             var duration = (endTime - startTime);
    //
    //             _requestStartTimes.Remove(trackingId);
    //
    //             NetworkRequestCompleted?.Invoke(new RaygunTimingEventArgs()
    //             {
    //                 Type = RaygunRumEventTimingType.NetworkCall,
    //                 Key = $"{request.Method} {request.RequestUri}",
    //                 Milliseconds = duration.Milliseconds
    //             });
    //
    //             break;
    //         }
    //     }
    // }
    //
    // public void OnError(Exception error)
    // {
    //     // Handle any errors here
    // }
    //
    // public void OnCompleted()
    // {
    //     // Handle completion here
    // }
    //
    // public bool ShouldIgnore(string url)
    // {
    //     return _settings.IgnoredViews != null && _settings.IgnoredUrls.Contains(url);
    // }

    private void OnNetworkRequestFinishedEvent(NetworkRequestFinished networkEvent)
    {
        NetworkRequestCompleted?.Invoke(new RaygunTimingEventArgs
        {
            Type = RaygunRumEventTimingType.NetworkCall,
            Key = networkEvent.Url,
            Milliseconds = networkEvent.Duration
        });
    }
}