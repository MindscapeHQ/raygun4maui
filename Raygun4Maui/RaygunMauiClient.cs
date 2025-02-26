using System.Reflection;
using Mindscape.Raygun4Net;
using System.Collections;
using Raygun4Maui.DeviceIdProvider;
using Raygun4Maui.MauiRUM;

namespace Raygun4Maui
{
    public class RaygunMauiClient : RaygunClient
    {
        private IDeviceIdProvider _deviceIdProvider;

        private readonly Raygun4MauiSettings _mauiSettings;

        public static RaygunMauiClient Current { get; private set; }

        private readonly RaygunMauiEnvironmentMessageBuilder _environmentMessageBuilder = new();

        private static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        //It does not seem like this can be obtained automatically
        private static readonly string ClientUrl = "https://github.com/MindscapeHQ/raygun4maui";

        public static readonly RaygunClientMessage ClientMessage = new()
        {
            Name = Name,
            Version = Version,
            ClientUrl = ClientUrl
        };

        internal static void Attach(RaygunMauiClient client)
        {
            if (Current != null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }

            Current = client;
        }

        public RaygunMauiClient(Raygun4MauiSettings settings, IRaygunUserProvider userProvider, IEnumerable<IMessageBuilder> messageBuilders) : base(
            settings.RaygunSettings, userProvider, messageBuilders)
        {
            _mauiSettings = settings;
        }

        public void EnableRealUserMonitoring(IDeviceIdProvider deviceIdProvider)
        {
            if (!_mauiSettings.EnableRealUserMonitoring)
            {
                return;
            }

            _deviceIdProvider = deviceIdProvider;

            var defaultUser = new RaygunIdentifierMessage(_deviceIdProvider.GetDeviceId()) { IsAnonymous = true };

            RaygunRum.Enable(_mauiSettings, defaultUser);
        }


        protected override async Task StripAndSend(Exception exception, IList<string> tags, IDictionary userCustomData,
            RaygunIdentifierMessage userInfo)
        {
            foreach (var e in StripWrapperExceptions(exception))
            {
                var msg = await BuildMessage(e, tags, userCustomData, userInfo, customiseMessage: raygunMessage =>
                {
                    raygunMessage.Details.MachineName = DeviceInfo.Current.Name;
                    raygunMessage.Details.Environment = _environmentMessageBuilder.BuildEnvironmentMessage(_settings);
                    raygunMessage.Details.Client = ClientMessage;
                }).ConfigureAwait(false);

                await Send(msg).ConfigureAwait(false);
            }
        }

        public override async Task SendInBackground(Exception exception, IList<string> tags = null,
            IDictionary userCustomData = null, RaygunIdentifierMessage userInfo = null)
        {
            if (CanSend(exception))
            {
                var exceptions = StripWrapperExceptions(exception);


                foreach (Exception ex in exceptions)
                {
                    var msg = await BuildMessage(ex, tags, userCustomData, userInfo, customiseMessage: raygunMessage =>
                    {
                        raygunMessage.Details.MachineName = DeviceInfo.Current.Name;
                        raygunMessage.Details.Environment =
                            _environmentMessageBuilder.BuildEnvironmentMessage(_settings);
                        raygunMessage.Details.Client = ClientMessage;
                    });


                    if (!Enqueue(msg))
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Could not add message to background queue. Dropping exception: {0}", ex);
                    }
                }

                FlagAsSent(exception);
            }
        }
    }
}