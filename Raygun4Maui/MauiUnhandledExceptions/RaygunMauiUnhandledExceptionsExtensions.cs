using System.Diagnostics;
using Raygun4Maui.MattJohnsonPint.Maui;
using Raygun4Net.BuildPlatforms;

namespace Raygun4Maui.MauiUnhandledExceptions
{
    internal static class RaygunMauiUnhandledExceptionsExtensions
    {
        internal static MauiAppBuilder AddRaygunUnhandledExceptionsListener(
            this MauiAppBuilder mauiAppBuilder
            )
        {
            MauiExceptions.CaptureWindowsUnhandeledExceptions();

            return mauiAppBuilder;
        }

        private static void AttachMauiExceptionHandler(Raygun4MauiSettings raygunMauiSettings)
        {
            MauiExceptions.UnhandledException += (sender, args) =>
            {
                try {
                    Exception e = (Exception)args.ExceptionObject;
                    List<string> tags = new List<string>() { "UnhandledException" };

                    if (raygunMauiSettings.RaygunSettings.SendDefaultTags)
                    {
                        Exception e = (Exception)args.ExceptionObject;
                        List<string> tags = new List<string>() { "UnhandledException" };

                      if (raygunMauiSettings.RaygunLoggerConfiguration.SendDefaultTags)
                    {
                        tags.Add(Raygun4NetBuildPlatforms.GetBuildPlatform());
                    }
                        
                        RaygunMauiClient.Current.SendInBackground(e, tags, null);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Unhandled exception handler had an error: {e.Message}");
                };
        }
    }
}