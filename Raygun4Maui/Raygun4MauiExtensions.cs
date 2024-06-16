#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.LifecycleEvents;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Breadcrumbs;
using Mindscape.Raygun4Net.Offline;
using Raygun4Maui.DeviceIdProvider;
using Raygun4Maui.MauiRUM.AppLifecycleHandlers;
using Raygun4Net.RaygunLogger;

namespace Raygun4Maui
{
    public static class Raygun4MauiExtensions
    {
        internal const string DeviceIdKey = "DeviceID";

        public static MauiAppBuilder AddRaygun(this MauiAppBuilder mauiAppBuilder, Raygun4MauiSettings raygunMauiSettings)
        {
            mauiAppBuilder.Services.AddSingleton(raygunMauiSettings);

            mauiAppBuilder.Services.AddSingleton<IRaygunMauiUserProvider, RaygunMauiUserProvider>();

            mauiAppBuilder.Services.AddSingleton<IRaygunUserProvider>(sp => sp.GetRequiredService<IRaygunMauiUserProvider>());

            mauiAppBuilder.Services.AddSingleton<IMauiInitializeService, RaygunClientInitializeService>();

            mauiAppBuilder.Services.AddSingleton(services => new RaygunMauiClient(raygunMauiSettings, services.GetService<IRaygunUserProvider>()));

            // Makes breadcrumbs have a global context
            RaygunBreadcrumbs.Storage = new InMemoryBreadcrumbStorage();

            if (raygunMauiSettings.EnableRealUserMonitoring)
            {
                mauiAppBuilder.AddRaygunRum();
            }

#if ANDROID
            var androidAssemblyReader = new Lazy<IAssemblyReader?>(AndroidUtilities.CreateAssemblyReader);
            RaygunErrorMessageBuilder.AssemblyReaderProvider = moduleName => androidAssemblyReader.Value?.TryGetReader(moduleName);
#endif

            return mauiAppBuilder
                .AddRaygunLogger(raygunMauiSettings.RaygunLoggerConfiguration);
        }

        /// <summary>
        /// This will attempt to pull in from the Raygun4MauiSettings section in your configuration file, if an options is provided it will apply that to the settings that we pull in.
        /// </summary>
        /// <param name="mauiAppBuilder"></param>
        /// <param name="options">Used to apply changes to a pulled in or default Raygun4MauiSettings object</param>
        /// <returns></returns>
        public static MauiAppBuilder AddRaygun(this MauiAppBuilder mauiAppBuilder, Action<Raygun4MauiSettings>? options = null)
        {
            var settings = mauiAppBuilder.Configuration.GetSection("Raygun4MauiSettings").Get<Raygun4MauiSettings>() ??
                           new Raygun4MauiSettings();

            options?.Invoke(settings);

            return mauiAppBuilder.AddRaygun(settings);
        }

        /// <summary>
        /// Allows the addition of an unmanaged IRaygunMauiUserProvider rather than the default Raygun implementation
        /// Note: If you are using RUM you, when you set the user you must create a RaygunAppEventPublisher event for the user update or else your sessions will not capture correctly.
        /// </summary>
        /// <param name="mauiAppBuilder"></param>
        /// <returns></returns>
        public static MauiAppBuilder AddRaygunUserProvider<T>(this MauiAppBuilder mauiAppBuilder) where T : class, IRaygunMauiUserProvider
        {
            mauiAppBuilder.Services.AddSingleton<IRaygunMauiUserProvider, T>();
            return mauiAppBuilder;
        }

        /// <summary>
        /// Sets the offline store, and a timer to attempt to send any offline crashes at the interval specified. Default 30 seconds.
        /// </summary>
        /// <param name="mauiSettings"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static Raygun4MauiSettings UseOfflineStorage(this Raygun4MauiSettings mauiSettings, TimeSpan? interval = null)
        {
            return mauiSettings.UseOfflineStorage(new TimerBasedSendStrategy(interval));
        }

        /// <summary>
        /// Sets the offline store, and the internal sending strategy to that specified
        /// </summary>
        /// <param name="mauiSettings"></param>
        /// <param name="backgroundSendStrategy"></param>
        /// <returns></returns>
        public static Raygun4MauiSettings UseOfflineStorage(this Raygun4MauiSettings mauiSettings, IBackgroundSendStrategy backgroundSendStrategy)
        {
            return mauiSettings.UseOfflineStorage(new RaygunMauiOfflineStore(backgroundSendStrategy));
        }

        /// <summary>
        /// Specify a custom implementation of the offline store to store any crash reports that could not be sent due to connectivity or server issues
        /// </summary>
        /// <param name="mauiSettings"></param>
        /// <param name="offlineStore"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Raygun4MauiSettings UseOfflineStorage<T>(this Raygun4MauiSettings mauiSettings, T offlineStore) where T : OfflineStoreBase
        {
            mauiSettings.RaygunSettings.OfflineStore = offlineStore;
            return mauiSettings;
        }

        private static MauiAppBuilder AddRaygunRum(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.AddDeviceIdProvider();

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

        private static MauiAppBuilder AddDeviceIdProvider(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IDeviceIdProvider, DeviceIdProvider.DeviceIdProvider>();

            if (Preferences.Get(DeviceIdKey, null) == null)
            {
                Preferences.Set(DeviceIdKey, Guid.NewGuid().ToString());
            }

            return mauiAppBuilder;
        }
    }
}