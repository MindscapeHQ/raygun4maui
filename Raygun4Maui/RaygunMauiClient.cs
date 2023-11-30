
using System.Reflection;
using Mindscape.Raygun4Net;
using System.Globalization;
using System.Collections;
using static System.String;

namespace Raygun4Maui
{
    public class RaygunMauiClient : RaygunClient
    {
        private static RaygunMauiClient Instance;
        public static RaygunMauiClient Current => Instance;

        private static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ??  "Unknown";
        private static readonly string ClientUrl = "https://github.com/MindscapeHQ/raygun4maui"; //It does not seem like this can be obtained automatically

        public static readonly RaygunClientMessage clientMessage = new()
        {
            Name = Name,
            Version = Version,
            ClientUrl = ClientUrl
        };

        public RaygunMauiClient(string apiKey) : base(apiKey) { }

        public RaygunMauiClient(RaygunSettings settings) : base(settings) { }


        internal static void Attach(RaygunMauiClient client)
        {
            if (Instance is not null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }

            Instance = client;
        }


        public override Task SendAsync(Exception exception, IList<string> tags, IDictionary userCustomData, RaygunIdentifierMessage userInfo = null)
        {
            Fire.AndForget(base.SendAsync(exception, tags, userCustomData, userInfo));
            return Task.CompletedTask;
        }

        public override Task SendInBackground(
            Exception exception,
            IList<string> tags = null,
            IDictionary userCustomData = null,
            RaygunIdentifierMessage userInfo = null)
        {

            Fire.AndForget(base.SendInBackground(exception, tags, userCustomData, userInfo));
            return Task.CompletedTask;
        }

        protected override async Task<RaygunMessage> BuildMessage(Exception exception, IList<string> tags, IDictionary userCustomData, RaygunIdentifierMessage userInfo)
        {
            DateTime now = DateTime.Now;
            var environment = new RaygunMauiEnvironmentMessage //Most likely should be static
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
                Client = clientMessage,
                Error = RaygunErrorMessageBuilder.Build(exception),
                UserCustomData = userCustomData,
                Tags = tags,
                Version = ApplicationVersion,
                User = userInfo ?? UserInfo ?? (!IsNullOrEmpty(User) ? new RaygunIdentifierMessage(User) : null),
                Environment = environment
            };

            var message = new RaygunMessage
            {
                OccurredOn = DateTime.UtcNow,
                Details = details
            };

            var customGroupingKey = await OnCustomGroupingKey(exception, message).ConfigureAwait(false);

            if (IsNullOrEmpty(customGroupingKey) == false)
            {
                message.Details.GroupingKey = customGroupingKey;
            }

            return message;
        }
    }
} 
