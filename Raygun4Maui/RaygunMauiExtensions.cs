using Raygun4Maui.MauiUnhandledExceptions;
using Raygun4Maui.RaygunLogger;

namespace Mindscape.Raygun4Maui
{
    public static class RaygunMauiExtensions
    {
        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            RaygunMauiSettings raygunMauiSettings)
        {
            return mauiAppBuilder
                .AddRaygunUnhandledExceptionsListener(raygunMauiSettings)
                .AddRaygunLogger(raygunMauiSettings);
        }

        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            string apiKey
            )
        {
            return mauiAppBuilder.AddRaygun4Maui(new RaygunMauiSettings() { ApiKey = apiKey});
        }
    }
}
