using Mindscape.Raygun4Net;

namespace Raygun4Maui.RaygunLogger
{
    public sealed class RaygunLoggerSettings : AbstractRaygunLogger
    {
        private readonly RaygunSettingsBase _settings;

        public RaygunLoggerSettings(RaygunSettingsBase settings)
        {
            _settings = settings;
        }   

        protected override RaygunClient RaygunClientFactory()
        {
            return new RaygunClient(_settings);
        }
    }
}
