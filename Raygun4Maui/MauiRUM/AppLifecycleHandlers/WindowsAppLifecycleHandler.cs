using System.Diagnostics;
using Microsoft.Maui.LifecycleEvents;
using Raygun4Maui.AppEvents;
#if WINDOWS
    using Microsoft.UI.Xaml;
    using Application = Microsoft.UI.Xaml.Application;
    using Window = Microsoft.UI.Xaml.Window;
    using WindowActivatedEventArgs = Microsoft.UI.Xaml.WindowActivatedEventArgs;
#endif


namespace Raygun4Maui.MauiRUM.AppLifecycleHandlers;

public class WindowsAppLifecycleHandler
{
#if WINDOWS
    public void OnLaunching(Application application, LaunchActivatedEventArgs eventArgs)
    {
        RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.AppInitialised);

        RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.ViewTimingStarted, new RaygunViewTimingEventArgs()
        {
            Type = RaygunAppEventType.ViewTimingStarted,
            Name = application.GetType().FullName,
            OccurredOn = DateTime.UtcNow.Ticks
        });
        LogEventToConsole($" {DateTime.UtcNow.Millisecond}");
        LogEventToConsole("Windows Application Launching! - INIT",
            new Dictionary<String, String> { { "app", application.ToString() }, { "args", eventArgs.ToString() } });
    }

    public void OnLaunched(Application application, LaunchActivatedEventArgs eventArgs)
    {
        RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.AppStarted);
        RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.ViewTimingFinished, new RaygunViewTimingEventArgs()
        {
            Type = RaygunAppEventType.ViewTimingFinished,
            Name = application.GetType().FullName,
            OccurredOn = DateTime.UtcNow.Ticks
        });
        LogEventToConsole($" {DateTime.UtcNow.Millisecond}");
        LogEventToConsole("Windows Application Launched! - START",
            new Dictionary<String, String> { { "app", application.ToString() }, { "args", eventArgs.ToString() } });
    }

    public void OnResumed(Window window)
    {
        RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.AppResumed);

        LogEventToConsole("Windows Application Resumed! - RESUME",
            new Dictionary<String, String> { { "window", window.ToString() } });
    }

    public void OnActivated(Window window, WindowActivatedEventArgs eventArgs)
    {
        if(eventArgs.WindowActivationState == WindowActivationState.Deactivated) {
              RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.AppPaused);
        }

        LogEventToConsole(eventArgs.WindowActivationState.ToString());
        LogEventToConsole("Windows Application Activated! - ACTIVATED",
            new Dictionary<String, String> { { "app", window.ToString() }, { "args", eventArgs.ToString() } });
    }

    public void OnVisibilityChanged(Window window, WindowVisibilityChangedEventArgs eventArgs)
    {
        if(!eventArgs.Visible) {
            RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.AppStopped);
        }

        LogEventToConsole(eventArgs.Visible.ToString() + $" {DateTime.UtcNow.Millisecond}");
        LogEventToConsole("Windows Application Visibility Changed!",
            new Dictionary<String, String> { { "app", window.ToString() }, { "args", eventArgs.ToString() } });
    }

    public void OnClosed(Window window, WindowEventArgs eventArgs)
    {
        // TODO: OnClosed occurs when a window is closed, this does not guarantee the application has closed. This is also an issue for other window based listeners
        RaygunAppEventPublisher.EventOccurred(RaygunAppEventType.AppStopped);

        LogEventToConsole("Windows Application Closed! - DESTROY",
            new Dictionary<String, String> { { "app", window.ToString() }, { "args", eventArgs.ToString() } });
    }

    private void LogEventToConsole(string eventName, IDictionary<string, string> properties = null)
    {
        System.Diagnostics.Debug.WriteLine($"Event: {eventName}");
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                System.Diagnostics.Debug.WriteLine($"   {prop.Key}: {prop.Value}");
            }
        }
    }
#endif
}

internal static class RaygunRumWindowExtensions
{
    public static ILifecycleBuilder RegisterWindowsRaygunRumEventHandlers(this ILifecycleBuilder builder)
    {
        /*** Windows Lifecycle Events *** (https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-8.0)
         * INIT    | OnLaunching         | Invoked before the native window has been created and activated
         * START   | OnLaunched          | Invoked when the native window is created and activated
         * RESUME  | OnResumed           | Invoked when Activated event raised, if the app is returning
         * PAUSE   | OnActivated         | Invoked when the Activated event if the app isn't resuming, check if deactivated
         * STOPPED | OnVisibilityChanged | Invoked when the visibility is changed, must check that visibility is set to false
         * DESTROY | OnClosed            | Invoked when the window has been closed
         ***/

#if WINDOWS
        var handler = new WindowsAppLifecycleHandler();

        builder.AddWindows(windows =>
        {
            // INIT
            windows.OnLaunching(handler.OnLaunching);
            // START
            windows.OnLaunched(handler.OnLaunched);

            // RESUME
            windows.OnResumed(handler.OnResumed);

            // PAUSE
            windows.OnActivated(handler.OnActivated);

            // STOPPED
            windows.OnVisibilityChanged(handler.OnVisibilityChanged);

            // DESTROY
            windows.OnClosed(handler.OnClosed);
        });
#endif

        return builder;
    }
}