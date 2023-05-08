using Mindscape.Raygun4Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Raygun4Maui
{
    internal class RaygunClientTest : RaygunClient
    {
        public RaygunClientTest(string apiKey) : base(apiKey)
        {
        }

        public RaygunClientTest(RaygunSettings settings) : base(settings)
        {
        }

        protected override async Task<RaygunMessage> BuildMessage(Exception exception, IList<string> tags, IDictionary userCustomData, RaygunIdentifierMessage userInfo)
        {

            DateTime now = DateTime.Now;

            var environment = new RaygunMauiEnvironmentMessage //can also mostlikely be static
            {
                UtcOffset = TimeZoneInfo.Local.GetUtcOffset(now).TotalHours,
                Locale = CultureInfo.CurrentCulture.DisplayName,
                OSVersion = DeviceInfo.Current.VersionString,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                WindowBoundsWidth = DeviceDisplay.MainDisplayInfo.Width,
                WindowBoundsHeight = DeviceDisplay.MainDisplayInfo.Height,
                DeviceManufacturer = DeviceInfo.Current.Manufacturer,
                OS = DeviceInfo.Current.Platform.ToString(),
            };

            var client = new RaygunClientMessage //This should be set statically 
            {
                Name = "Raygun4Maui",
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                ClientUrl = "https://github.com/MindscapeHQ/raygun4maui"
            };

            var details = new RaygunMessageDetails
            {
                MachineName = Environment.MachineName,
                Client = client,
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
