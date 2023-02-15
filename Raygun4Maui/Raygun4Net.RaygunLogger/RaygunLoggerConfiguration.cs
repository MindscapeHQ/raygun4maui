using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Net;

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
