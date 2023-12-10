using System.Reflection;

namespace Raygun4Maui.AppEvents;

public class RaygunAppEventPublisher
{
    private static RaygunAppEventPublisher _publisher;

    public static RaygunAppEventPublisher Instance
    {
        get
        {
            if (_publisher == null)
            {
                _publisher = new RaygunAppEventPublisher();
            }

            return _publisher;
        }
    }

    private static List<Action<AppInitialised>> _appInitialisedDelegates;
    private static List<Action<AppStarted>> _appStartedDelegates;
    private static List<Action<AppPaused>> _appPausedDelegates;
    private static List<Action<AppResumed>> _appResumedDelegates;
    private static List<Action<AppStopped>> _appStoppedDelegates;
    private static List<Action<AppDestroyed>> _appDestroyedDelegates;
    private static List<Action<ViewTimingStarted>> _appViewTimingStartedDelegates;
    private static List<Action<ViewTimingFinished>> _appViewTimingFinishedDelegates;


    private RaygunAppEventPublisher()
    {
        _appInitialisedDelegates = new List<Action<AppInitialised>>();
        _appStartedDelegates = new List<Action<AppStarted>>();
        _appPausedDelegates = new List<Action<AppPaused>>();
        _appResumedDelegates = new List<Action<AppResumed>>();
        _appStoppedDelegates = new List<Action<AppStopped>>();
        _appDestroyedDelegates = new List<Action<AppDestroyed>>();
        _appViewTimingStartedDelegates = new List<Action<ViewTimingStarted>>();
        _appViewTimingFinishedDelegates = new List<Action<ViewTimingFinished>>();
    }

    public static void Subscribe<T>(Action<T> callback)
    {
        _publisher ??= new RaygunAppEventPublisher();
        
        var eventType = typeof(T);

        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        if (eventType == typeof(AppInitialised))
        {
            _appInitialisedDelegates.Add(callback as Action<AppInitialised>);
        }
        else if (eventType == typeof(AppStarted))
        {
            _appStartedDelegates.Add(callback as Action<AppStarted>);
        }
        else if (eventType == typeof(AppPaused))
        {
            _appPausedDelegates.Add(callback as Action<AppPaused>);
        }
        else if (eventType == typeof(AppResumed))
        {
            _appResumedDelegates.Add(callback as Action<AppResumed>);
        }
        else if (eventType == typeof(AppStopped))
        {
            _appStoppedDelegates.Add(callback as Action<AppStopped>);
        }
        else if (eventType == typeof(AppDestroyed))
        {
            _appDestroyedDelegates.Add(callback as Action<AppDestroyed>);
        }
        else if (eventType == typeof(ViewTimingStarted))
        {
            _appViewTimingStartedDelegates.Add(callback as Action<ViewTimingStarted>);
        }
        else if (eventType == typeof(ViewTimingFinished))
        {
            _appViewTimingFinishedDelegates.Add(callback as Action<ViewTimingFinished>);
        }
    }

    public static void Publish(IRaygunAppEvent eventInfo)
    {
        _publisher ??= new RaygunAppEventPublisher();

        switch (eventInfo)
        {
            case AppInitialised info:
                NotifyDelegates(_appInitialisedDelegates, info);
                break;
            case AppStarted info:
                NotifyDelegates(_appStartedDelegates, info);
                break;
            case AppPaused info:
                NotifyDelegates(_appPausedDelegates, info);
                break;
            case AppResumed info:
                NotifyDelegates(_appResumedDelegates, info);
                break;
            case AppStopped info:
                NotifyDelegates(_appStoppedDelegates, info);
                break;
            case AppDestroyed info:
                NotifyDelegates(_appDestroyedDelegates, info);
                break;
            case ViewTimingStarted info:
                NotifyDelegates(_appViewTimingStartedDelegates, info);
                break;
            case ViewTimingFinished info:
                NotifyDelegates(_appViewTimingFinishedDelegates, info);
                break;
        }
    }

    private static void NotifyDelegates<T>(List<Action<T>> delegates, T eventInfo)
    {
        if (delegates == null) return;

        foreach (var delegateCallback in delegates)
        {
            delegateCallback(eventInfo);
        }
    }
}