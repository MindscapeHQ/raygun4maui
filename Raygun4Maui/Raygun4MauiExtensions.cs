#nullable enable
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.LifecycleEvents;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Breadcrumbs;
using Mindscape.Raygun4Net.Offline;
using Raygun4Maui.DeviceIdProvider;
using Raygun4Maui.MauiRUM.AppLifecycleHandlers;

namespace Raygun4Maui
{
    public static class Raygun4MauiExtensions
    {
        internal const string DeviceIdKey = "DeviceID";

        public static MauiAppBuilder AddRaygun(this MauiAppBuilder mauiAppBuilder, Raygun4MauiSettings raygunMauiSettings)
        {
            mauiAppBuilder.Services.AddSingleton(raygunMauiSettings);

            mauiAppBuilder.Services.AddSingleton<IRaygunMauiUserProvider, RaygunMauiUserProvider>();
            
            mauiAppBuilder.Services.AddSingleton<IMessageBuilder, AppVersionMessageBuilder>();

            mauiAppBuilder.Services.AddSingleton<IRaygunUserProvider>(sp => sp.GetRequiredService<IRaygunMauiUserProvider>());

            mauiAppBuilder.Services.AddSingleton<IMauiInitializeService, RaygunClientInitializeService>();

            // Resolved on first use, so that a user provider or message builder depending on ILogger does not form a cycle through the Raygun logger, which needs this client
            mauiAppBuilder.Services.AddSingleton(services => new RaygunMauiClient(raygunMauiSettings, new LazyRaygunUserProvider(() => services.GetService<IRaygunUserProvider>()), new LazyMessageBuilders(() => services.GetServices<IMessageBuilder>())));

            // Lets Raygun4Net packages that depend on the base client, such as Mindscape.Raygun4Net.Extensions.Logging, resolve the same instance
            mauiAppBuilder.Services.AddSingleton<RaygunClientBase>(services => services.GetRequiredService<RaygunMauiClient>());

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

            return mauiAppBuilder;
        }

        /// <summary>
        /// This will attempt to pull in from the Raygun4MauiSettings section in your configuration file, if an options is provided it will apply that to the settings that we pull in.
        /// </summary>
        /// <param name="mauiAppBuilder"></param>
        /// <param name="options">Used to apply changes to a pulled in or default Raygun4MauiSettings object</param>
        /// <returns></returns>
        public static MauiAppBuilder AddRaygun(this MauiAppBuilder mauiAppBuilder, Action<Raygun4MauiSettings>? options = null)
        {
            var section = mauiAppBuilder.Configuration.GetSection("Raygun4MauiSettings");

            // Without this check, a config from before 4.0.0 loads with no ApiKey and Raygun silently sends nothing
            if (section.GetSection("RaygunSettings").Exists())
            {
                throw new InvalidOperationException("Raygun4MauiSettings:RaygunSettings was removed in Raygun4Maui 4.0.0. Move its settings up into Raygun4MauiSettings, for example Raygun4MauiSettings:ApiKey.");
            }

            // Without this check, a config from before 4.0.0 loads with no built-in logger and Raygun silently stops sending ILogger logs
            if (section.GetSection("RaygunLoggerConfiguration").Exists())
            {
                throw new InvalidOperationException("Raygun4MauiSettings:RaygunLoggerConfiguration was removed in Raygun4Maui 4.0.0. Remove the section, then to keep sending ILogger logs add Mindscape.Raygun4Net.Extensions.Logging and call builder.Logging.AddRaygunLogger().");
            }

            var settings = section.Get<Raygun4MauiSettings>() ?? new Raygun4MauiSettings();

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
        /// Allows the addition of a IMessageBuilder implementation which can modify messages before
        /// being sent to Raygun. Multiple can be registered.
        /// </summary>
        /// <param name="mauiAppBuilder"></param>
        /// <typeparam name="T">Implemented type of IMessageBuilder</typeparam>
        /// <returns></returns>
        public static MauiAppBuilder AddRaygunMessageBuilder<T>(this MauiAppBuilder mauiAppBuilder) where T : class, IMessageBuilder
        {
            mauiAppBuilder.Services.AddSingleton<IMessageBuilder, T>();
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
            mauiSettings.OfflineStore = offlineStore;
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