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

public static class RaygunNetworkTracker
{
    private static Raygun4MauiSettings _settings;

    private static List<string> _defaultIgnoredUrls;

    private const string NetworkTimingIntentAction = "com.raygun.networkmonitorlibrary.NetworkRequestTiming";

    public static event Action<RaygunTimingEventArgs> NetworkRequestCompleted;

#if WINDOWS
    private static RaygunWindowsNetworkMonitor _windowsNetworkMonitor;
#elif ANDROID
    private static RaygunAndroidNativeNetworkTracker _androidNetworkMonitor;
    private static RaygunAndroidNetworkReceiver _androidNetworkReceiver;
#elif IOS || MACCATALYST
    private static RaygunAppleNativeNetworkMonitor _appleNetworkMonitor;
    private static RaygunAppleNativeNetworkObserver _appleNetworkObserver;
#endif

    private static void OnAppStarted(AppStarted obj)
    {
#if ANDROID
        if (!_settings.RumFeatureFlags.HasFlag(RumFeatures.Network))
        {
            return;
        }
        
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

    public static void Init(Raygun4MauiSettings settings)
    {
        RaygunAppEventPublisher.AppStarted += OnAppStarted;

        _settings = settings;
        
        _defaultIgnoredUrls = new List<string>()
            { _settings.RumApiEndpoint.Host, _settings.RaygunSettings.ApiEndpoint.Host };


        if (!_settings.RumFeatureFlags.HasFlag(RumFeatures.Network))
        {
            return;
        }
        
        RaygunAppEventPublisher.NetworkRequestFinished += OnNetworkRequestFinishedEvent;

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

    public static bool ShouldIgnore(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        var isUrlInSettingsIgnored = _settings?.IgnoredUrls?.Contains(url) ?? false;
        var isUrlInDefaultIgnored = _defaultIgnoredUrls?.Contains(url) ?? false;

        return isUrlInSettingsIgnored || isUrlInDefaultIgnored;
    }


    private static void OnNetworkRequestFinishedEvent(NetworkRequestFinished networkEvent)
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