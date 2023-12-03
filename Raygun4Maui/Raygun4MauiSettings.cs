using Raygun4Maui.Raygun4Net.RaygunLogger;

namespace Raygun4Maui
{
    public sealed class Raygun4MauiSettings : RaygunLoggerConfiguration
    {
        public Uri RumApiEndpoint { get; set; }
        public IList<string> IgnoredViews { get; set; }
        public IList<string> IgnoredUrls { get; set; }
    }
}