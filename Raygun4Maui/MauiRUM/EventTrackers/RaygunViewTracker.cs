using Mindscape.Raygun4Net;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTrackers.Apple;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public static class RaygunViewTracker
{
    public static event Action<RaygunTimingEventArgs> ViewLoaded;

    private static readonly Dictionary<string, long> Timers = new();
    private static DateTime _previousPageDisappearingTime;

#if IOS || MACCATALYST
    private static RaygunUiViewControllerObserver _appleRaygunUiViewControllerObserver;
#endif
    
    private static Raygun4MauiSettings _settings;

    public static void Init(Raygun4MauiSettings settings)
    {
        _settings = settings;

        if (!_settings.RumFeatureFlags.HasFlag(RumFeatures.Page))
        {
            return;
        }
        
        RaygunAppEventPublisher.ViewTimingStarted += OnViewTimingStarted;
        RaygunAppEventPublisher.ViewTimingFinished += OnViewTimingFinished;

        // Set up page listeners when application is available
        RaygunAppEventPublisher.AppStarted += SetupPageDelegates;

#if IOS || MACCATALYST
        // Native timings only make sense if you have page timings as well, so we early return if there are no page timings
        if (!_settings.RumFeatureFlags.HasFlag(RumFeatures.AppleNativeTimings))
        {
            return;
        }
        
        _appleRaygunUiViewControllerObserver = new RaygunUiViewControllerObserver();
        _appleRaygunUiViewControllerObserver.Register();
#endif
    }


    private static void SetupPageDelegates(AppStarted args)
    {
        if (Application.Current == null) return;

        RaygunAppEventPublisher.AppStarted -= SetupPageDelegates;

        Application.Current.PageAppearing += OnPageAppearing;
        Application.Current.PageDisappearing += OnPageDisappearing;
    }

    private static void OnViewTimingStarted(ViewTimingStarted viewEvent)
    {
        Timers.TryAdd(viewEvent.Name, viewEvent.OccurredOn);
    }

    private static void OnViewTimingFinished(ViewTimingFinished viewEvent)
    {
        if (!Timers.Remove(viewEvent.Name, out var value)) return;

        if (!ShouldIgnore(viewEvent.Name))
        {
            InvokeViewLoadedEvent(viewEvent.Name, GetDuration(value, viewEvent.OccurredOn));
        }

        SetPageLoadTimeStart();
    }

    private static void OnPageDisappearing(object sender, Page page)
    {
        if (page is NavigationPage)
        {
            return;
        }

        SetPageLoadTimeStart();
    }


    private static void OnPageAppearing(object sender, Page page)
    {
        var pageName = page.GetType().Name;

        if (page is NavigationPage || ShouldIgnore(pageName))
        {
            return;
        }

        if (_previousPageDisappearingTime != DateTime.MinValue)
        {
            InvokeViewLoadedEvent(pageName, GetDuration(_previousPageDisappearingTime.Ticks, DateTime.UtcNow.Ticks));
        }
    }


    private static void SetPageLoadTimeStart()
    {
        _previousPageDisappearingTime = DateTime.UtcNow;
    }


    private static long GetDuration(long startTicks, long finishTicks)
    {
        return (long)TimeSpan.FromTicks(finishTicks - startTicks).TotalMilliseconds;
    }

    public static bool ShouldIgnore(string viewName)
    {
        return _settings.IgnoredViews != null && _settings.IgnoredViews.Contains(viewName);
    }

    private static void InvokeViewLoadedEvent(string name, long duration)
    {
        ViewLoaded?.Invoke(new RaygunTimingEventArgs
        {
            Type = RaygunRumEventTimingType.ViewLoaded,
            Key = name,
            Milliseconds = duration
        });
    }
}