namespace Raygun4Maui.AppEvents;

public enum RaygunAppEventType
{
    // App lifecycle events
    AppInitialised,
    AppStarted,
    AppResumed,
    AppPaused,
    AppStopped,
    AppDestroyed,

    // View events
    ViewTimingStarted,
    ViewTimingFinished,

    // Network events
    NetworkRequestOccurred,
}