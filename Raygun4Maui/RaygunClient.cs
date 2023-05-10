using Mindscape.Raygun4Net;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace Raygun4Maui
{
    public class RaygunClient : Mindscape.Raygun4Net.RaygunClient
    {
        public RaygunClient(string apiKey) : base(apiKey)
        {
        }

        public RaygunClient(RaygunSettings settings) : base(settings)
        {
        }

        public static readonly RaygunClientMessage ClientMessage = new()
        {
            Name = "Raygun4Maui",
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            ClientUrl = "https://github.com/MindscapeHQ/raygun4maui"
        };


        protected override async Task<RaygunMessage> BuildMessage(Exception exception, IList<string> tags, IDictionary userCustomData, RaygunIdentifierMessage userInfo)
        {
            DateTime now = DateTime.Now;
            var environment = new RaygunMauiEnvironmentMessage //Mostlikely should be static
            {
                UtcOffset = TimeZoneInfo.Local.GetUtcOffset(now).TotalHours,
                Locale = CultureInfo.CurrentCulture.DisplayName,
                OSVersion = DeviceInfo.Current.VersionString,
                Architecture = NativeDeviceInfo.Architecture(),
                WindowBoundsWidth = DeviceDisplay.MainDisplayInfo.Width,
                WindowBoundsHeight = DeviceDisplay.MainDisplayInfo.Height,
                DeviceManufacturer = DeviceInfo.Current.Manufacturer,
                Platform = NativeDeviceInfo.Platform(),
                Model = DeviceInfo.Current.Model,
                ProcessorCount = Environment.ProcessorCount,
                ResolutionScale = DeviceDisplay.MainDisplayInfo.Density,
                TotalPhysicalMemory = NativeDeviceInfo.TotalPhysicalMemory(),
                AvailablePhysicalMemory = NativeDeviceInfo.AvailablePhysicalMemory(),
                CurrentOrientation = DeviceDisplay.MainDisplayInfo.Orientation.ToString(),
            };
           
            var details = new RaygunMessageDetails
            {
                MachineName = DeviceInfo.Current.Name,
                Client = ClientMessage,
                Error = RaygunErrorMessageBuilder.Build(exception),
                UserCustomData = userCustomData,
                Tags = tags,
                Version = ApplicationVersion,
                User = userInfo ?? UserInfo ?? (!String.IsNullOrEmpty(User) ? new RaygunIdentifierMessage(User) : null),
                Environment = environment
            };

            var message = new RaygunMessage
            {
                OccurredOn = DateTime.UtcNow,
                Details = details
            };

            var customGroupingKey = await OnCustomGroupingKey(exception, message).ConfigureAwait(false);

            if (string.IsNullOrEmpty(customGroupingKey) == false)
            {
                message.Details.GroupingKey = customGroupingKey;
            }

            return message;
        }
    }
}
