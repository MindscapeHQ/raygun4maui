using System.Diagnostics;
using Raygun4Maui.MattJohnsonPint.Maui;
using Raygun4Net.BuildPlatforms;

namespace Raygun4Maui.MauiUnhandledExceptions
{
    internal static class RaygunMauiUnhandledExceptionsExtensions
    {
        internal static MauiAppBuilder AddRaygunUnhandledExceptionsListener(
            this MauiAppBuilder mauiAppBuilder,
            Raygun4MauiSettings raygunMauiSettings
        )
        {
            AttachMauiExceptionHandler(raygunMauiSettings);

            return mauiAppBuilder;
        }

        private static void AttachMauiExceptionHandler(Raygun4MauiSettings raygunMauiSettings)
        {
            MauiExceptions.UnhandledException += (sender, args) =>
            {
                try
                {
                    Exception e = (Exception)args.ExceptionObject;
                    List<string> tags = new List<string>() { "UnhandledException" };

                    if (raygunMauiSettings.SendDefaultTags)
                    {
                        tags.Add(Raygun4NetBuildPlatforms.GetBuildPlatform());
                    }
                    
                    RaygunMauiClient.Current.Send(e, tags, null);
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Unhandled exception handler had an error: {e.Message}");
                }
            };
        }
    }
}