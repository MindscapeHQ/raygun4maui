using Mindscape.Raygun4Maui;
using Raygun4Maui.MattJohnsonPint.Maui;

namespace Raygun4Maui.MauiUnhandledExceptions
{
    internal static class RaygunMauiUnhandledExceptionsExtensions
    {
        internal static MauiAppBuilder AddRaygunUnhandledExceptionsListener(
            this MauiAppBuilder mauiAppBuilder,
            RaygunMauiSettings raygunMauiSettings
            )
        {
            AttachMauiExceptionHandler(raygunMauiSettings);

            return mauiAppBuilder;
        }

        private static void AttachMauiExceptionHandler(RaygunMauiSettings raygunMauiSettings)
        {
            MauiExceptions.UnhandledException += (sender, args) =>
            {
                Exception e = (Exception)args.ExceptionObject;
                List<string> tags = new List<string>() { "UnhandledException" };

                if (raygunMauiSettings.SendDefaultTags)
                {
                    tags.Add(RaygunMauiClient.GetBuildPlatform());
                }

                (new RaygunMauiClient(raygunMauiSettings)).Send(e, tags, null);
            };
        }
    }
}
