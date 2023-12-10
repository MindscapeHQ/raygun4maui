using Mindscape.Raygun4Net;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunViewTracker
{
    public event EventHandler<RaygunTimingEventArgs> ViewLoaded;

    private readonly Dictionary<string, long> _timers;
    private DateTime _previousPageDisappearingTime;

    private Raygun4MauiSettings _settings;

    public RaygunViewTracker()
    {
        _timers = new Dictionary<string, long>();
    }

    public void Init(Raygun4MauiSettings settings)
    {
        _settings = settings;

        RaygunAppEventPublisher.Subscribe<ViewTimingStarted>(OnViewTimingStarted);
        RaygunAppEventPublisher.Subscribe<ViewTimingFinished>(OnViewTimingFinished);


        // Set up page listeners when application is available
        if (_settings.RumFeatureFlags.HasFlag(RumFeatures.Page))
        {
            RaygunAppEventPublisher.Subscribe<AppStarted>(SetupPageDelegates);
        }
    }


    private void SetupPageDelegates(AppStarted args)
    {
        if (Application.Current == null) return;

        Application.Current.PageAppearing += OnPageAppearing;
        Application.Current.PageDisappearing += OnPageDisappearing;
    }

    private void OnViewTimingStarted(ViewTimingStarted viewEvent)
    {
        _timers.TryAdd(viewEvent.Name, viewEvent.OccurredOn);
    }

    private void OnViewTimingFinished(ViewTimingFinished viewEvent)
    {
        if (_timers.ContainsKey(viewEvent.Name))
        {
            var start = _timers[viewEvent.Name];

            _timers.Remove(viewEvent.Name);

            if (!ShouldIgnore(viewEvent.Name))
            {
                InvokeViewLoadedEvent(viewEvent.Name, GetDuration(start, viewEvent.OccurredOn));
            }

            SetPageLoadTimeStart();
        }
    }

    private void OnPageDisappearing(object sender, Page page)
    {
        if (page is NavigationPage)
        {
            return;
        }

        SetPageLoadTimeStart();
    }


    private void OnPageAppearing(object sender, Page page)
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


    private void SetPageLoadTimeStart()
    {
        _previousPageDisappearingTime = DateTime.UtcNow;
    }


    private long GetDuration(long startTicks, long finishTicks)
    {
        return (long)TimeSpan.FromTicks(finishTicks - startTicks).TotalMilliseconds;
    }

    public bool ShouldIgnore(string viewName)
    {
        return _settings.IgnoredViews != null && _settings.IgnoredViews.Contains(viewName);
    }

    private void InvokeViewLoadedEvent(string name, long duration)
    {
        ViewLoaded?.Invoke(this, new RaygunTimingEventArgs
        {
            Type = RaygunRumEventTimingType.ViewLoaded,
            Key = name,
            Milliseconds = duration
        });
    }
}