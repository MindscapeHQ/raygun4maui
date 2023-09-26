using Microsoft.Maui.LifecycleEvents;
using Raygun4Maui.MauiRum.EventTracking;

namespace Raygun4Maui.MauiRum.ApplicationLifecycle
{
    internal static class RaygunRumWindowExtensions
    {
        public static ILifecycleBuilder RegisterWindowsRaygunRumEventHandlers(this ILifecycleBuilder builder, IRaygunRumEventTracker raygunRumEventTracker)
        {
#if WINDOWS
            builder.AddWindows(windows =>
            {
                windows.OnActivated((app, args) => {
                    raygunRumEventTracker.TrackEvent("Windows Application Activated!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });

                windows.OnClosed((app, args) => {
                    raygunRumEventTracker.TrackEvent("Windows Application Closed!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });

                windows.OnLaunched((app, args) =>
                {
                    raygunRumEventTracker.TrackEvent("Windows Application Launched!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });

                windows.OnLaunching((app, args) => {
                    raygunRumEventTracker.TrackEvent("Windows Application Launching!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });

                /*windows.OnPlatformMessage((app, args) => {
                    raygunRumEventTracker.TrackEvent("Windows Application Platform Message!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });*/

                /*windows.OnPlatformWindowSubclassed((app, args) => {
                    raygunRumEventTracker.TrackEvent("Windows Application Platform Window Subclassed!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });*/

                windows.OnResumed(window => {
                    raygunRumEventTracker.TrackEvent("Windows Application Resumed!", new Dictionary<String, String> { { "window", window.ToString() }});
                });

                windows.OnVisibilityChanged((app, args) => {
                    raygunRumEventTracker.TrackEvent("Windows Application Visibility Changed!", new Dictionary<String, String> { { "app", app.ToString() }, { "args", args.ToString() } });
                });

                windows.OnWindowCreated(window => {
                    raygunRumEventTracker.TrackEvent("Windows Application Window Created!", new Dictionary<String, String> { { "window", window.ToString() } });
                });

            });
#endif

            return builder;
        }
    }
}
