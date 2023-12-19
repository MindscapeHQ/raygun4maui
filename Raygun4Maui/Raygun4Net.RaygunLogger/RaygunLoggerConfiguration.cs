using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Net;

// Namespace is different, doesn't include Raygun4Maui because we may want to move this to Raygun4Net
// ReSharper disable once CheckNamespace
namespace Raygun4Net.RaygunLogger
{
    public class RaygunLoggerConfiguration : RaygunSettings
    {
        public bool SendDefaultTags{get; set;}

        public bool SendDefaultCustomData { get; set;}

        public LogLevel MinLogLevel {get; set;}
        public LogLevel MaxLogLevel { get; set; }

        public RaygunLoggerConfiguration()
        {
            SendDefaultTags = true;
            SendDefaultCustomData = true;
            MinLogLevel = LogLevel.Debug;
            MaxLogLevel = LogLevel.Critical;
        }
    }
}
