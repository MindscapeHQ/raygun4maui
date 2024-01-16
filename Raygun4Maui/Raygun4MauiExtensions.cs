﻿using System.Diagnostics;
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

            Console.WriteLine($"Checking RUM enable {raygunMauiSettings.EnableRealUserMonitoring}");
            if (raygunMauiSettings.EnableRealUserMonitoring)
            {
                mauiAppBuilder.AddRaygunRum();
            }
            
            return mauiAppBuilder
                .AddRaygunUnhandledExceptionsListener()
                .AddRaygunLogger(raygunMauiSettings.RaygunLoggerConfiguration);
        }

        /// <summary>
        /// This will attempt to pull in form the Raygun4MauiSettings section in your configuration file, if an options is provided it will apply that to the settings that we pull in.
        /// If there the configuration is not provided we create a default Raygun4MauiSettings object
        /// </summary>
        /// <param name="mauiAppBuilder"></param>
        /// <param name="options">Used to apply changes to a pulled in or default Raygun4MauiSettings object</param>
        /// <returns></returns>
        public static MauiAppBuilder AddRaygun(this MauiAppBuilder mauiAppBuilder,
            Action<Raygun4MauiSettings> options = null)
        {
            Console.WriteLine("Adding Raygun");
            var settings = mauiAppBuilder.Configuration.GetSection("Raygun4MauiSettings").Get<Raygun4MauiSettings>() ??
                           new Raygun4MauiSettings();
            
            

            options?.Invoke(settings);

            return mauiAppBuilder.AddRaygun(settings);
        }
        
        private static MauiAppBuilder AddRaygunRum(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.AddDeviceIdProvider();

            var deviceIdProvider = new DeviceIdProvider.DeviceIdProvider();

            Console.WriteLine("Enabling RUM");
            RaygunMauiClient.Current.EnableRealUserMonitoring(deviceIdProvider);

            mauiAppBuilder.ConfigureLifecycleEvents(builder =>
            {
#if WINDOWS
                builder.RegisterWindowsRaygunRumEventHandlers();
#elif ANDROID
                builder.RegisterAndroidLifecycleHandler();
#elif IOS || MACCATALYST
                builder.RegisterAppleRaygunRumEventHandlers();

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