#if IOS || MACCATALYST
using Foundation;
using UIKit;
#endif
using Microsoft.Maui.LifecycleEvents;
using Raygun4Maui.AppEvents;


namespace Raygun4Maui.MauiRUM.AppLifecycleHandlers;

public class AppleAppLifecycleHandler
{
#if IOS || MACCATALYST
    public bool OnWillFinishLaunching(UIApplication application, NSDictionary launchOptions)
    {
        RaygunAppEventPublisher.Publish(new AppInitialised());
        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
        {
            Name = application.GetType().FullName,
            OccurredOn = DateTime.UtcNow.Ticks
        });

        return true;
    }

    public bool OnFinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        RaygunAppEventPublisher.Publish(new AppStarted());
        RaygunAppEventPublisher.Publish(new ViewTimingFinished()
        {
            Name = application.GetType().FullName,
            OccurredOn = DateTime.UtcNow.Ticks
        });

        return true;
    }

    public void OnActivated(UIApplication application)
    {
        RaygunAppEventPublisher.Publish(new AppResumed());
    }

    public void OnResignActivation(UIApplication application)
    {
        RaygunAppEventPublisher.Publish(new AppPaused());
    }

    public void OnWillEnterForeground(UIApplication application)
    {
        RaygunAppEventPublisher.Publish(new AppStarted());
    }

    public void OnDidEnterBackground(UIApplication application)
    {
        RaygunAppEventPublisher.Publish(new AppStopped());
    }

    public void OnWillTerminate(UIApplication application)
    {
        RaygunAppEventPublisher.Publish(new AppDestroyed());
    }


#endif
}

internal static class RaygunRumAppleExtensions
{
    public static ILifecycleBuilder RegisterAppleRaygunRumEventHandlers(this ILifecycleBuilder builder)
    {
        /*** Apple Lifecycle Events *** (https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-8.0)
         * INIT    | WillFinishLaunching | Invoked when app launching has begun, but state restoration has not yet occurred.
         * START   | FinishedLaunching   | Invoked when the app has launched.
         * RESUME  | OnActivated         | Invoked when the app is launched and every time the app returns to the foreground.
         * PAUSE   | OnResignActivation  | Invoked when the app is about to enter the background, be suspended, or when the user receives an interruption such as a phone call or text.
         * STOPPED | DidEnterBackground  | Invoked when the app has entered the background.
         * RESTART | WillEnterForeground | Invoked if the app will be returning from a backgrounded state.
         * DESTROY | WillTerminate       | Invoked if the app is being terminated due to memory constraints, or directly by the user.
         ***/

#if IOS || MACCATALYST
        var handler = new AppleAppLifecycleHandler();

        builder.AddiOS(apple =>
        {
            // INIT
            apple.WillFinishLaunching(handler.OnWillFinishLaunching);

            // START
            apple.FinishedLaunching(handler.OnFinishedLaunching);

            // RESUME
            apple.OnActivated(handler.OnActivated);

            // PAUSE
            apple.OnResignActivation(handler.OnResignActivation);

            // STOPPED
            apple.DidEnterBackground(handler.OnDidEnterBackground);

            // RESTART
            apple.WillEnterForeground(handler.OnWillEnterForeground);

            // DESTROY
            apple.WillTerminate(handler.OnWillTerminate);
        });
#endif

        return builder;
    }
}