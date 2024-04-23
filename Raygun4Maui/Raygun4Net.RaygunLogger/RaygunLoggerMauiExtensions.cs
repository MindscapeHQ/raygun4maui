using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

// Namespace is different, doesn't include Raygun4Maui because we may want to move this to Raygun4Net
// ReSharper disable once CheckNamespace
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
