using Mindscape.Raygun4Net;

namespace Raygun4Maui.RaygunLogger
{
    public sealed class RaygunLoggerSettings : RaygunSettings
    {
        public bool SendDefaultTags{get; set;}

        public bool SendDefaultCustomData { get; set;}

        public RaygunLoggerSettings()
        {
            SendDefaultTags = true;
            SendDefaultCustomData = true;
        }
    }
}
