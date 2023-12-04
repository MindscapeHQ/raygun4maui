using System.Reflection;
using Mindscape.Raygun4Net;
using System.Globalization;
using System.Collections;
using Microsoft.Extensions.Options;
using Raygun4Maui.DeviceIdProvider;
using Raygun4Maui.MauiRUM;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui
{
    public class RaygunMauiClient : RaygunClient
    {
        private RaygunRum _rum;

        public override RaygunIdentifierMessage UserInfo
        {
            get => _userInfo;
            set
            {
                _userInfo = value;
                _rum?.UpdateUser(value);
            }
        }

        private RaygunIdentifierMessage _userInfo;

        private IDeviceIdProvider _deviceId;

        private static RaygunMauiClient _instance;
        private Raygun4MauiSettings _mauiSettings;
        public static RaygunMauiClient Current => _instance;

        private readonly Lazy<RaygunMauiEnvironmentMessageBuilder> _lazyMessageBuilder =
            new Lazy<RaygunMauiEnvironmentMessageBuilder>(RaygunMauiEnvironmentMessageBuilder.Init);

        private RaygunMauiEnvironmentMessageBuilder EnvironmentMessageBuilder => _lazyMessageBuilder.Value;

        private static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static readonly string
            ClientUrl =
                "https://github.com/MindscapeHQ/raygun4maui"; //It does not seem like this can be obtained automatically

        public static readonly RaygunClientMessage ClientMessage = new()
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

        public RaygunMauiClient(IOptions<Raygun4MauiSettings> settings) : base(settings.Value.RaygunSettings)
        {
            _rum = new RaygunRum();
            _mauiSettings = settings.Value;
        }

        public RaygunMauiClient(string apiKey) : base(apiKey)
        {
            _rum = new RaygunRum();
        }

        public RaygunMauiClient(Raygun4MauiSettings settings) : base(settings.RaygunSettings)
        {
            _rum = new RaygunRum();
            _mauiSettings = settings;
        }

        public void EnableRealUserMonitoring(IDeviceIdProvider deviceId)
        {
            // TODO: Find a better way to inject deviceId
            _deviceId = deviceId;

            _userInfo = new RaygunIdentifierMessage(_deviceId.GetDeviceId()) { IsAnonymous = true };

            _rum.Enable(_mauiSettings, _userInfo);
        }

        public void SendTimingEvent(RaygunRumEventTimingType type, string name, long milliseconds)
        {
            if (_rum.Enabled)
            {
                _rum.SendCustomTimingEvent(type, name, milliseconds);
            }
        }

        protected override async Task<RaygunMessage> BuildMessage(Exception exception, IList<string> tags,
            IDictionary userCustomData, RaygunIdentifierMessage userInfo)
        {
            var environment = EnvironmentMessageBuilder.BuildEnvironmentMessage();

            var details = new RaygunMessageDetails
            {
                MachineName = DeviceInfo.Current.Name,
                Client = ClientMessage,
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