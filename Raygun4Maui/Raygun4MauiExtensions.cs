using Mindscape.Raygun4Net;
using Raygun4Maui.MauiUnhandledExceptions;
using Raygun4Net.RaygunLogger;

namespace Raygun4Maui
{
    public static class Raygun4MauiExtensions
    {
        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            Raygun4MauiSettings raygunMauiSettings)
        {
            RaygunMauiClient.Attach(new RaygunClient(raygunMauiSettings));
            return mauiAppBuilder
                .AddRaygunUnhandledExceptionsListener(raygunMauiSettings)
                .AddRaygunLogger(raygunMauiSettings);
        }

        public static MauiAppBuilder AddRaygun4Maui(
            this MauiAppBuilder mauiAppBuilder,
            string apiKey
            )
        {
            return mauiAppBuilder.AddRaygun4Maui(new Raygun4MauiSettings() { ApiKey = apiKey });
        }
    }
}
