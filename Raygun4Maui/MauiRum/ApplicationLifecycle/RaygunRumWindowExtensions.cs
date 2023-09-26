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
                windows.OnLaunched((app, args) =>
                {
                    raygunRumEventTracker.TrackEvent("Windows application launched!");
                });
            });
#endif

            return builder;
        }
    }
}
