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

        private readonly Lazy<RaygunMauiEnvironmentMessageBuilder> _lazyMessageBuilder =
            new Lazy<RaygunMauiEnvironmentMessageBuilder>(RaygunMauiEnvironmentMessageBuilder.Init);

        private RaygunMauiEnvironmentMessageBuilder EnvironmentMessageBuilder => _lazyMessageBuilder.Value;

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
        }


        protected override async Task<RaygunMessage> BuildMessage(Exception exception, IList<string> tags,
            IDictionary userCustomData, RaygunIdentifierMessage userInfo)
        {
            var environment = EnvironmentMessageBuilder.BuildEnvironmentMessage();

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