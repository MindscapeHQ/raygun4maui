using Microsoft.Extensions.Logging;

namespace Raygun4Maui.RaygunLogger
{
    public sealed class RaygunLoggerConfiguration
    {
        public Dictionary<LogLevel, RaygunLoggerSettings> LogLevelToRaygunSettings { get; set; } = new()
        {
            
        };
    }
}