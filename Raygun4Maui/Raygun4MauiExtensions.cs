using Microsoft.Maui.LifecycleEvents;
using Mindscape.Raygun4Net;
using Raygun4Maui.DeviceIdProvider;
using Raygun4Maui.MauiUnhandledExceptions;
using Raygun4Maui.MauiRUM.AppLifecycleHandlers;
using Raygun4Maui.Raygun4Net.RaygunLogger;

namespace Raygun4Maui
{

    public static class Raygun4MauiExtensions
    {
        internal const string DeviceIdKey = "DeviceID";

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
            return mauiAppBuilder.AddRaygun4Maui(new Raygun4MauiSettings() { ApiKey = apiKey, RumApiEndpoint = new Uri("https://api.raygun.com/events")});
        }

        public static MauiAppBuilder EnableRaygunRum(this MauiAppBuilder mauiAppBuilder)
        {
            // Configured device id provider to have device based UUID for users
            mauiAppBuilder.AddDeviceIdProvider();
            
            // TODO: Figure out how to actually get an instance instead of creating it here
            var deviceIdProvider = new DeviceIdProvider.DeviceIdProvider();
            
            RaygunMauiClient.Current.EnableRealUserMonitoring(deviceIdProvider);
            
            mauiAppBuilder.ConfigureLifecycleEvents(builder =>
            {
                

#if WINDOWS
            builder.RegisterWindowsRaygunRumEventHandlers();
#elif IOS || MACCATALYST
                builder.AddiOS(ios => {});
#elif ANDROID
                builder.AddAndroid(android => {});

#endif
            });

            return mauiAppBuilder;
        }

        private static MauiAppBuilder AddDeviceIdProvider(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IDeviceIdProvider, DeviceIdProvider.DeviceIdProvider>();
            
            if (Preferences.Get(DeviceIdKey, null) == null)
                Preferences.Set(DeviceIdKey, Guid.NewGuid().ToString());

            return builder;
        }
    }
}
