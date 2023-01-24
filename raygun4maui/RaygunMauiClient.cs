using Mindscape.Raygun4Net;

namespace Mindscape.Raygun4Maui
{
    // All the code in this file is included in all platforms.
    public class RaygunMauiClient : RaygunClient
    {
        public RaygunMauiClient(string apiKey) : base(apiKey)
        {
            this.RaygunMauiClientInit();
        }

        public RaygunMauiClient(RaygunSettingsBase settings) : base(settings)
        {
            this.RaygunMauiClientInit();
        }

        private void RaygunMauiClientInit()
        {
            AttachMauiExceptionHandler();
        }

        private void AttachMauiExceptionHandler()
        {
            MattJohnsonPint.Maui.MauiExceptions.UnhandledException += (sender, args) =>
            {
                Exception e = (Exception)args.ExceptionObject;
                this.Send(e);
            };
        }
    }
}