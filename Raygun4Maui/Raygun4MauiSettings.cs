using Raygun4Maui.Raygun4Net.RaygunLogger;

namespace Raygun4Maui
{
    [Flags]
    public enum RumFeatures
    {
        None = 0,
        Network = 1 << 1,
        Page = 1 << 2,
        AppleNativeTimings = 1 << 3
    }
    
    public sealed class Raygun4MauiSettings
    {
        private static readonly Uri DefaultRumApiEndpoint = new Uri("https://api.raygun.com/events");
        public RaygunLoggerConfiguration RaygunSettings { get; set; }
        public Uri RumApiEndpoint { get; set; } = DefaultRumApiEndpoint;
        public IList<string> IgnoredViews { get; set; }
        public IList<string> IgnoredUrls { get; set; }

        public bool EnableRealUserMonitoring { get; set; } = true;
        
        public RumFeatures RumFeatureFlags { get; set; }


        public Raygun4MauiSettings(RaygunLoggerConfiguration raygunSettings)
        {
            RaygunSettings = raygunSettings;
        }

        public Raygun4MauiSettings(string apiKey)
        {
            RaygunSettings = new RaygunLoggerConfiguration() {ApiKey = apiKey};
        }
        
        public Raygun4MauiSettings() {}
    }
}