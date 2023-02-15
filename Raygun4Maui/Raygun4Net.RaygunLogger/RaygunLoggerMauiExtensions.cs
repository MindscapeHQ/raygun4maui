using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Raygun4Net.RaygunLogger
{
    public static class RaygunLoggerMauiExtensions
    {
        internal static MauiAppBuilder AddRaygunLogger(
            this MauiAppBuilder mauiAppBuilder,
            RaygunLoggerConfiguration raygunLoggerConfiguration)
        {
            mauiAppBuilder.Logging.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ILoggerProvider), new RaygunLoggerProvider(raygunLoggerConfiguration)));

            return mauiAppBuilder;
        }
    }
}
