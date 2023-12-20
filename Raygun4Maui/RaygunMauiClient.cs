using System.Reflection;
using Mindscape.Raygun4Net;
using System.Globalization;
using System.Collections;

namespace Raygun4Maui
{
    public class RaygunMauiClient : RaygunClient
    {
        private static RaygunMauiClient _instance;
        public static RaygunMauiClient Current => _instance;

        private static readonly object EnvironmentLock = new();

        private static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;

        private static readonly string Version =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

        private static readonly string
            ClientUrl =
                "https://github.com/MindscapeHQ/raygun4maui"; //It does not seem like this can be obtained automatically

        public static readonly RaygunClientMessage clientMessage = new()
        {
            Name = Name,
            Version = Version,
            ClientUrl = ClientUrl
        };

        internal static void Attach(RaygunMauiClient client)
        {
            if (_instance != null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }

            _instance = client;
        }

        public RaygunMauiClient(string apiKey) : base(apiKey)
        {
        }

        public RaygunMauiClient(RaygunSettings settings) : base(settings)
        {
            System.Diagnostics.Debug.WriteLine("RAYGUN4MAUI CREATED");
        }

        protected override async Task<RaygunMessage> BuildMessage(Exception exception, IList<string> tags,
            IDictionary userCustomData, RaygunIdentifierMessage userInfo)
        {
            RaygunMauiEnvironmentMessage environment;
            lock (EnvironmentLock)
            {
                DateTime now = DateTime.Now;

                environment = new RaygunMauiEnvironmentMessage //Mostlikely should be static
                {
                    // Combination of these cause the error, it may happen with the other ones too
                    UtcOffset = TimeZoneInfo.Local.GetUtcOffset(now).TotalHours, // Not this,
                    Locale = CultureInfo.CurrentCulture.DisplayName, // Not this,
                    OSVersion = DeviceInfo.Current.VersionString, // Not This,
                    Architecture = NativeDeviceInfo.Architecture(), // Not This,
                    WindowBoundsWidth = DeviceDisplay.MainDisplayInfo.Width, // <------ THIS!!
                    
                    
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
            }

            var details = new RaygunMessageDetails
            {
                MachineName = DeviceInfo.Current.Name,
                Client = clientMessage,
                Error = RaygunErrorMessageBuilder.Build(exception),
                UserCustomData = userCustomData,
                Tags = tags,
                Version = ApplicationVersion,
                User = userInfo ??
                       UserInfo ?? (!String.IsNullOrEmpty(User) ? new RaygunIdentifierMessage(User) : null),
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