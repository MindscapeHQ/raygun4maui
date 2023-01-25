using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Raygun4Maui.RaygunLogger
{
    public static class RaygunLoggerExtensions
    {
        public static ILoggingBuilder AddRaygunLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, RaygunLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <RaygunLoggerConfiguration, RaygunLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddRaygunLogger(
            this ILoggingBuilder builder,
            Action<RaygunLoggerConfiguration> configure)
        {
            builder.AddRaygunLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
