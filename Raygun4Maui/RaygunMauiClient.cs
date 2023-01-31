using Microsoft.Extensions.Hosting;
using Mindscape.Raygun4Net;

namespace Mindscape.Raygun4Maui
{
    // All the code in this file is included in all platforms.
    public class RaygunMauiClient : RaygunClient
    {
        public RaygunMauiClient(string apiKey) : base(apiKey)
        {
            
        }

        public RaygunMauiClient(RaygunSettingsBase settings) : base(settings)
        {
            
        }

        public static string GetBuildPlatform()
        {
#if WINDOWS
            return "Windows";
#elif IOS
            return  "iOS";
#elif MACCATALYST
            return "MacCatalyst";
#elif ANDROID
            return "Android";
#else
            return "UknownBuildPlatform";
#endif
        }
    }
}