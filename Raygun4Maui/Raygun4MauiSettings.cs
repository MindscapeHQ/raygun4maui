using Raygun4Maui.Raygun4Net.RaygunLogger;

namespace Raygun4Maui
{
    public sealed class Raygun4MauiSettings
    {
        private static readonly Uri DefaultRumApiEndpoint = new Uri("https://api.raygun.com/events");
        public RaygunLoggerConfiguration RaygunSettings { get; set; }
        public Uri RumApiEndpoint { get; set; } = DefaultRumApiEndpoint;
        public IList<string> IgnoredViews { get; set; }
        public IList<string> IgnoredUrls { get; set; }

        public bool EnableRealUserMonitoring { get; set; } = true;


        public Raygun4MauiSettings(RaygunLoggerConfiguration raygunSettings)
        {
            RaygunSettings = raygunSettings;
        }

        public Raygun4MauiSettings(string apiKey)
        {
            RaygunSettings = new RaygunLoggerConfiguration() {ApiKey = apiKey};
        }
    }
}