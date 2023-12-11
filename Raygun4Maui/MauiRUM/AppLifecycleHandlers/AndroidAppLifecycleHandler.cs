#if ANDROID
using Android.App;
using Android.OS;
using Raygun4Maui.AppEvents;
#endif
using Microsoft.Maui.LifecycleEvents;

namespace Raygun4Maui.MauiRUM.AppLifecycleHandlers;

public class AndroidAppLifecycleHandler
{
#if ANDROID
    public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
    {
        RaygunAppEventPublisher.Publish(new AppInitialised());
        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
            {
                Name = GetName(activity),
                OccurredOn = DateTime.UtcNow.Ticks
            }
        );
    }

    public void OnActivityStarted(Activity activity)
    {
        RaygunAppEventPublisher.Publish(new AppStarted());
        RaygunAppEventPublisher.Publish(new ViewTimingStarted()
        {
            Name = GetName(activity),
            OccurredOn = DateTime.UtcNow.Ticks
        });
    }

    public void OnActivityResumed(Activity activity)
    {
        RaygunAppEventPublisher.Publish(new AppResumed());
        RaygunAppEventPublisher.Publish(new ViewTimingFinished()
        {
            Name = GetName(activity),
            OccurredOn = DateTime.UtcNow.Ticks
        });
    }

    public void OnActivityPaused(Activity activity)
    {
        RaygunAppEventPublisher.Publish(new AppPaused());
    }

    public void OnActivityStopped(Activity activity)
    {
        RaygunAppEventPublisher.Publish(new AppStopped());
    }

    public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
    {
        RaygunAppEventPublisher.Publish(new AppStopped());
    }

    public void OnActivityDestroyed(Activity activity)
    {
        RaygunAppEventPublisher.Publish(new AppDestroyed());
    }

    private static string GetName(Activity activity)
    {
        return activity.GetType().FullName;
    }
#endif
}

internal static class RaygunRumAndroidExtensions
{
    public static ILifecycleBuilder RegisterAndroidLifecycleHandler(this ILifecycleBuilder builder)
    {
        /*** Android Lifecycle Events *** (https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-8.0)
         * INIT    | OnCreate            | Invoked when the app is starting, before an activity, service, or receiver objects (excluding content providers) have been created.
         * START   | OnStart             | Invoked after OnCreate or OnRestart when the activity has been stopped, but is now being displayed to the user.
         * RESUME  | OnResumed           | Invoked after OnRestoreInstanceState, OnRestart, or OnPause, to indicate that the activity is active and is ready to receive input.
         * PAUSE   | OnPause             | Invoked when an activity is going into the background, but has not yet been killed.
         * STOPPED | OnStop              | Invoked when the activity is no longer visible to the user.
         * STOPPED | OnSaveInstanceState | Invoked to retrieve per-instance state from an activity being killed so that the state can be restored in OnCreate or OnRestoreInstanceState.
         * DESTROY | OnDestroy           | Invoked when the activity is finishing, or because the system is temporarily destroying the activity instance to save space.
         ***/

#if ANDROID
        var handler = new AndroidAppLifecycleHandler();

        builder.AddAndroid(android =>
        {
            // INIT
            android.OnCreate(handler.OnActivityCreated);
            // START
            android.OnStart(handler.OnActivityStarted);

            // RESUME
            android.OnResume(handler.OnActivityResumed);

            // PAUSE
            android.OnPause(handler.OnActivityPaused);

            // STOPPED
            android.OnStop(handler.OnActivityStopped);
            android.OnSaveInstanceState(handler.OnActivitySaveInstanceState);

            // DESTROY
            android.OnDestroy(handler.OnActivityDestroyed);
        });
#endif

        return builder;
    }
}