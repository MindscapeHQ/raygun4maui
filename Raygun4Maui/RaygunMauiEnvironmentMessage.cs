using Mindscape.Raygun4Net;

namespace Raygun4Maui
{
    internal class RaygunMauiEnvironmentMessage : RaygunEnvironmentMessage
    {
        public double ResolutionScale { get; set; }
        public string CurrentOrientation { get; set; }
        public string DeviceManufacturer { get; set; }
        public string Model { get; set; }
        public string DeviceName { get; set; }
        public string Platform { get; set; }
    }
}
