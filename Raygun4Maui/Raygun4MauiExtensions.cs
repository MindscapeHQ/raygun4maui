using Microsoft.Extensions.Configuration;
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

        public static MauiAppBuilder AddRaygun(
            this MauiAppBuilder mauiAppBuilder,
            Raygun4MauiSettings raygunMauiSettings)
        {
            var client = new RaygunMauiClient(raygunMauiSettings);

            mauiAppBuilder.Services.AddSingleton<RaygunMauiClient>(client);
            RaygunMauiClient.Attach(client);

            if (raygunMauiSettings.EnableRealUserMonitoring)
            {
                mauiAppBuilder.AddRaygunRum();
            }

            return mauiAppBuilder
                .AddRaygunUnhandledExceptionsListener(raygunMauiSettings)
                .AddRaygunLogger(raygunMauiSettings.RaygunSettings);
        }

        public static MauiAppBuilder AddRaygun(this MauiAppBuilder mauiAppBuilder,
            Action<Raygun4MauiSettings> options = null)
        {
            var settings = mauiAppBuilder.Configuration.GetSection("Raygun4MauiSettings").Get<Raygun4MauiSettings>();

            options?.Invoke(settings);

            return mauiAppBuilder.AddRaygun(settings);
        }

        [Obsolete("Method is deprecated, this will not enable RUM")]
        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            Raygun4MauiSettings raygunMauiSettings)
        {
            RaygunMauiClient.Attach(new RaygunMauiClient(raygunMauiSettings));

            return mauiAppBuilder
                .AddRaygunUnhandledExceptionsListener(raygunMauiSettings)
                .AddRaygunLogger(raygunMauiSettings.RaygunSettings);
        }


        [Obsolete("Method is deprecated, this will not enable RUM")]
        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            string apiKey
        )
        {
            return mauiAppBuilder.AddRaygun(new Raygun4MauiSettings(apiKey));
        }

        private static MauiAppBuilder AddRaygunRum(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.AddDeviceIdProvider();

            var deviceIdProvider = new DeviceIdProvider.DeviceIdProvider();

            RaygunMauiClient.Current.EnableRealUserMonitoring(deviceIdProvider);

            mauiAppBuilder.ConfigureLifecycleEvents(builder =>
            {
#if WINDOWS
                builder.RegisterWindowsRaygunRumEventHandlers();
#elif ANDROID
                builder.RegisterAndroidLifecycleHandler();
#elif IOS || MACCATALYST
                builder.AddiOS(ios => {});

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