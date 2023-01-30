using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Configuration;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.RaygunLogger
{
    public static class RaygunLoggerExtensions
    {
        public static MauiAppBuilder AddRaygun4Maui(this MauiAppBuilder mauiAppBuilder)
        {

            mauiAppBuilder.Services.AddOptions<RaygunLoggerConfiguration>().BindConfiguration(nameof(RaygunLoggerConfiguration));

            mauiAppBuilder.Logging.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, RaygunLoggerProvider>());

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
