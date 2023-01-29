using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Configuration;

namespace Raygun4Maui.RaygunLogger
{
    public static class RaygunLoggerExtensions
    {
        public static MauiAppBuilder AddRaygun4Maui(this MauiAppBuilder mauiAppBuilder)
        {
             
            //mauiAppbuilder.AddConfiguration();
     
            mauiAppBuilder.Services.TryAdd(ServiceDescriptor.Singleton<ILoggerProvider, RaygunLoggerProvider>());
            mauiAppBuilder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(RaygunLoggerProvider)));

            LoggerProviderOptions.RegisterProviderOptions
                <RaygunLoggerConfiguration, RaygunLoggerProvider>(mauiAppBuilder.Services);
            
            /*
            mauiAppBuilder.Services.AddSingleton<RaygunLoggerProvider>();
            mauiAppBuilder.Services.AddSingleton<MauiApp>();
            */
            
            return mauiAppBuilder;
        }

        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder builder,
            Action<RaygunLoggerConfiguration> configure)
        {
            builder.AddRaygun4Maui();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
