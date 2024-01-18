using System.Diagnostics;
using System.Reflection;
using Microsoft.Maui.Platform;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTrackers.Apple;
using Raygun4Maui.MauiRUM.EventTrackers.Windows;

#if ANDROID
using Android.Content;
using Com.Raygun.Networkmonitorlibrary;
using Raygun4Maui.MauiRUM.EventTrackers.Android;
#endif

using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunNetworkTracker
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

#if WINDOWS
    private RaygunWindowsNetworkMonitor _windowsNetworkMonitor;
#elif ANDROID
    private RaygunAndroidNativeNetworkTracker _androidNetworkMonitor;
    private RaygunAndroidNetworkReceiver _androidNetworkReceiver;
#elif IOS
    private RaygunAppleNativeNetworkMonitor _appleNetworkMonitor;
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
        
        _androidNetworkReceiver = new RaygunAndroidNetworkReceiver();
        IntentFilter filter = new IntentFilter(NETWORK_TIMING_INTENT_ACTION);

        // activity?.RegisterReceiver(androidReciever, filter);
        
        activity?.ApplicationContext?.RegisterReceiver(_androidNetworkReceiver, filter);
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

        Trace.WriteLine("Right here :)");
        RaygunAppEventPublisher.Instance.NetworkRequestFinished += OnNetworkRequestFinishedEvent;

        if (settings.RumFeatureFlags.HasFlag(RumFeatures.Network))
        {
#if WINDOWS
            Trace.WriteLine("PLuh!!!!");
            _windowsNetworkMonitor = new RaygunWindowsNetworkMonitor();
            DiagnosticListener.AllListeners.Subscribe(_windowsNetworkMonitor);
#elif ANDROID
            _androidNetworkMonitor = new RaygunAndroidNativeNetworkTracker();
            _androidNetworkMonitor.Enable();
#elif IOS
            _appleNetworkMonitor = new RaygunAppleNativeNetworkMonitor();
            _appleNetworkMonitor.Enable();

            _appleNetworkObserver = new RaygunAppleNativeNetworkObserver();
            _appleNetworkObserver.Register();
#endif
        }
    }

    public bool ShouldIgnore(string url)
    {
        return _settings.IgnoredUrls != null && _settings.IgnoredUrls.Contains(url);
    }

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