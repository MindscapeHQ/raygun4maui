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

    public event Action<AppInitialised> AppInitialised;
    public event Action<AppStarted> AppStarted;
    public event Action<AppPaused> AppPaused;
    public event Action<AppResumed> AppResumed;
    public event Action<AppStopped> AppStopped;
    public event Action<AppDestroyed> AppDestroyed;
    public event Action<ViewTimingStarted> ViewTimingStarted;
    public event Action<ViewTimingFinished> ViewTimingFinished;


    private RaygunAppEventPublisher()
    {
    }

    public static void Publish(IRaygunAppEvent eventInfo)
    {
        _publisher ??= new RaygunAppEventPublisher();

        switch (eventInfo)
        {
            case AppInitialised info:
                Instance.AppInitialised(info);
                break;
            case AppStarted info:
                Instance.AppStarted(info);
                break;
            case AppPaused info:
                Instance.AppPaused(info);
                break;
            case AppResumed info:
                Instance.AppResumed(info);
                break;
            case AppStopped info:
                Instance.AppStopped(info);
                break;
            case AppDestroyed info:
                Instance.AppDestroyed(info);
                break;
            case ViewTimingStarted info:
                Instance.ViewTimingStarted(info);
                break;
            case ViewTimingFinished info:
                Instance.ViewTimingFinished(info);
                break;
        }
    }
}