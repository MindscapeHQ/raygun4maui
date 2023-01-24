using Mindscape.Raygun4Net;

namespace Mindscape.Raygun4Maui
{
    // All the code in this file is included in all platforms.
    public class RaygunMauiClient : RaygunClient
    {
        public RaygunMauiClient(string apiKey) : base(apiKey)
        {
            this._raygunMauiClientInit();
        }

        public RaygunMauiClient(RaygunSettingsBase settings) : base(settings)
        {
            this._raygunMauiClientInit();
        }

        private void _raygunMauiClientInit()
        {

        }
    }
}