using Raygun4Maui.MauiUnhandledExceptions;
using Raygun4Net.RaygunLogger;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raygun4Maui.MauiRum.EventTracking;
using Raygun4Maui.MauiRum.ApplicationLifecycle;

namespace Raygun4Maui
{
    public static class Raygun4MauiExtensions
    {
        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            Raygun4MauiSettings raygunMauiSettings)
        {
            RaygunMauiClient.Attach(new RaygunMauiClient(raygunMauiSettings));
            return mauiAppBuilder
                .AddRaygunUnhandledExceptionsListener(raygunMauiSettings)
                .AddRaygunLogger(raygunMauiSettings);
        }

        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            string apiKey
            )
        {
            return mauiAppBuilder.AddRaygun4Maui(new Raygun4MauiSettings() { ApiKey = apiKey });
        }


        public static MauiAppBuilder EnableRaygunRum(this MauiAppBuilder mauiAppBuilder)
        {
            IRaygunRumEventTracker raygunRumEventTracker = new RaygunRumEventTracker();

            mauiAppBuilder.ConfigureLifecycleEvents(builder =>
            {
#if WINDOWS
                builder.RegisterWindowsRaygunRumEventHandlers(raygunRumEventTracker);
#elif IOS
                builder.AddiOS(ios =>
                {
                
                });

#elif ANDROID
                builder.AddAndroid(android =>
                {

                });
#endif
            });

            return mauiAppBuilder;
        }
    }
}
