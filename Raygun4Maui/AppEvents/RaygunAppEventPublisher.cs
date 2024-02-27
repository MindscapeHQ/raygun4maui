using System.Reflection;

namespace Raygun4Maui.AppEvents;

// TODO: Make static
public static class RaygunAppEventPublisher
{
    public static event Action<AppInitialised> AppInitialised;
    public static event Action<AppStarted> AppStarted;
    public static event Action<AppPaused> AppPaused;
    public static event Action<AppResumed> AppResumed;
    public static event Action<AppStopped> AppStopped;
    public static event Action<AppDestroyed> AppDestroyed;
    public static event Action<ViewTimingStarted> ViewTimingStarted;
    public static event Action<ViewTimingFinished> ViewTimingFinished;
    public static event Action<NetworkRequestFinished> NetworkRequestFinished;
    
    public static void Publish(IRaygunAppEvent eventInfo)
    {
        switch (eventInfo)
        {
            case AppInitialised info:
                AppInitialised?.Invoke(info);
                break;
            case AppStarted info:
                AppStarted?.Invoke(info);
                break;
            case AppPaused info:
                AppPaused?.Invoke(info);
                break;
            case AppResumed info:
                AppResumed?.Invoke(info);
                break;
            case AppStopped info:
                AppStopped?.Invoke(info);
                break;
            case AppDestroyed info:
                AppDestroyed?.Invoke(info);
                break;
            case ViewTimingStarted info:
                ViewTimingStarted?.Invoke(info);
                break;
            case ViewTimingFinished info:
                ViewTimingFinished?.Invoke(info);
                break;
            case NetworkRequestFinished info:
                NetworkRequestFinished?.Invoke(info);
                break;
        }
    }
}