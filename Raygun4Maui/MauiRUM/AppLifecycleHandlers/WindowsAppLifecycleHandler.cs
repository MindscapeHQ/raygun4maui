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
        RaygunAppEventPublisher.Publish(new AppInitialised());
        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
        {
            Name = application.GetType().FullName,
            OccurredOn = DateTime.UtcNow.Ticks
        });
    }

    public void OnLaunched(Application application, LaunchActivatedEventArgs eventArgs)
    {
        RaygunAppEventPublisher.Publish(new AppStarted());
        RaygunAppEventPublisher.Publish(new ViewTimingFinished()
        {
            Name = application.GetType().FullName,
            OccurredOn = DateTime.UtcNow.Ticks
        });
    }

    public void OnResumed(Window window)
    {
        RaygunAppEventPublisher.Publish(new AppResumed());
    }

    public void OnActivated(Window window, WindowActivatedEventArgs eventArgs)
    {
        if (eventArgs.WindowActivationState == WindowActivationState.Deactivated)
        {
            RaygunAppEventPublisher.Publish(new AppPaused());
        }
    }

    public void OnVisibilityChanged(Window window, WindowVisibilityChangedEventArgs eventArgs)
    {
        if (!eventArgs.Visible)
        {
            RaygunAppEventPublisher.Publish(new AppStopped());
        }
    }

    public void OnClosed(Window window, WindowEventArgs eventArgs)
    {
        RaygunAppEventPublisher.Publish(new AppDestroyed());
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