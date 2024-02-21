using System.Diagnostics;
using System.Reflection;
using Microsoft.Maui.Platform;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTrackers.Apple;
using Raygun4Maui.MauiRUM.EventTrackers.Windows;

#if ANDROID
using Android.Content;
using Android.OS;
using AndroidX.Core.Content;
using Com.Raygun.Networkmonitorlibrary;
using Raygun4Maui.MauiRUM.EventTrackers.Android;
#endif

using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunNetworkTracker
{
    private Raygun4MauiSettings _settings;
    
    private List<string> _defaultIgnoredUrls;

    private const string NetworkTimingIntentAction = "com.raygun.networkmonitorlibrary.NetworkRequestTiming";
    
    public event Action<RaygunTimingEventArgs> NetworkRequestCompleted;

#if WINDOWS
    private RaygunWindowsNetworkMonitor _windowsNetworkMonitor;
#elif ANDROID
    private RaygunAndroidNativeNetworkTracker _androidNetworkMonitor;
    private RaygunAndroidNetworkReceiver _androidNetworkReceiver;
#elif IOS || MACCATALYST
    private RaygunAppleNativeNetworkMonitor _appleNetworkMonitor;
    private RaygunAppleNativeNetworkObserver _appleNetworkObserver;
#endif

    public RaygunNetworkTracker()
    {
        RaygunAppEventPublisher.Instance.AppStarted += OnAppStarted;
    }

    private void OnAppStarted(AppStarted obj)
    {
#if ANDROID
        if (!_settings.RumFeatureFlags.HasFlag(RumFeatures.Network)) return;
        
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

        _androidNetworkReceiver = new RaygunAndroidNetworkReceiver();
        var filter = new IntentFilter(NetworkTimingIntentAction);


        ContextCompat.RegisterReceiver(activity?.ApplicationContext!, _androidNetworkReceiver, filter,
            ContextCompat.ReceiverExported);

        try
        {
            RaygunNetworkMonitor.SetContext(activity?.ApplicationContext);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to pass the context to the network monitor due to: {e.Message}");
        }
#endif
    }
    
    public void Init(Raygun4MauiSettings settings)
    {
        _defaultIgnoredUrls = new List<string>() { settings.RumApiEndpoint.Host, settings.RaygunSettings.ApiEndpoint.Host };

        _settings = settings;

        RaygunAppEventPublisher.Instance.NetworkRequestFinished += OnNetworkRequestFinishedEvent;

        if (settings.RumFeatureFlags.HasFlag(RumFeatures.Network))
        {
#if WINDOWS
            _windowsNetworkMonitor = new RaygunWindowsNetworkMonitor();
            DiagnosticListener.AllListeners.Subscribe(_windowsNetworkMonitor);
#elif ANDROID
            _androidNetworkMonitor = new RaygunAndroidNativeNetworkTracker();
            _androidNetworkMonitor.Enable();
#elif IOS || MACCATALYST
            _appleNetworkMonitor = new RaygunAppleNativeNetworkMonitor();
            _appleNetworkMonitor.Enable();

            _appleNetworkObserver = new RaygunAppleNativeNetworkObserver();
            _appleNetworkObserver.Register();
#endif
        }
    }

    public bool ShouldIgnore(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        var isUrlInSettingsIgnored = _settings?.IgnoredUrls?.Contains(url) ?? false;
        var isUrlInDefaultIgnored = _defaultIgnoredUrls?.Contains(url) ?? false;

        return isUrlInSettingsIgnored || isUrlInDefaultIgnored;
    }


    private void OnNetworkRequestFinishedEvent(NetworkRequestFinished networkEvent)
    {
        if (ShouldIgnore(networkEvent.Url))
        {
            return;
        }

        NetworkRequestCompleted?.Invoke(new RaygunTimingEventArgs
        {
            Type = RaygunRumEventTimingType.NetworkCall,
            Key = $"{networkEvent.Method} {networkEvent.Url}",
            Milliseconds = networkEvent.Duration
        });
    }
}