using System.Reflection;
using Mindscape.Raygun4Net;
using System.Globalization;
using System.Collections;
using Microsoft.Extensions.Options;
// using Mindscape.Raygun4Net.Breadcrumbs;
using Raygun4Maui.DeviceIdProvider;
using Raygun4Maui.MauiRUM;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui
{
    public class RaygunMauiClient : RaygunClient
    {
        public override RaygunIdentifierMessage UserInfo // Need to make sure this is set correctly with RUM still
        {
            get => _userInfo;
            set
            {
                _userInfo = value;
                RaygunRum.UpdateUser(value);
            }
        }

        private RaygunIdentifierMessage _userInfo;

        private IDeviceIdProvider _deviceIdProvider;

        private readonly Raygun4MauiSettings _mauiSettings;
        
        public static RaygunMauiClient Current { get; private set; }

        private readonly Lazy<RaygunMauiEnvironmentMessageBuilder> _lazyMessageBuilder =
            new(RaygunMauiEnvironmentMessageBuilder.Init);

        private RaygunMauiEnvironmentMessageBuilder EnvironmentMessageBuilder => _lazyMessageBuilder.Value;

        private static readonly string Name = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

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
            if (Current != null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }

            Current = client;
        }

        // TODO: Will this actually be used, and how does it work with IRaygunUserProvider
        public RaygunMauiClient(IOptions<Raygun4MauiSettings> settings) : base(settings.Value.RaygunSettings, null)
        {
            _mauiSettings = settings.Value;
        }

        public RaygunMauiClient(Raygun4MauiSettings settings, IRaygunUserProvider userProvider) : base(
            settings.RaygunSettings, userProvider)
        {
            _mauiSettings = settings;

            SendingMessage += (_, e) =>
            {
                e.Message.Details.Tags ??= new List<string>();
                
                foreach (var tag in BuildTags())
                {
                    e.Message.Details.Tags.Add(tag);
                }
            };
        }

        internal IEnumerable<string> BuildTags()
        {
            yield return $"Tag: 1";
            yield return $"Tag: {DeviceInfo.DeviceType}";
            yield return "Tag: 2 - CSharp";
        }

        // Should this really be a RaygunClient feature?
        public void EnableRealUserMonitoring(IDeviceIdProvider deviceIdProvider)
        {
            if (!_mauiSettings.EnableRealUserMonitoring) return;
            
            _deviceIdProvider = deviceIdProvider;

            // Still need to arrange how this should be dealt with
            _userInfo = new RaygunIdentifierMessage(_deviceIdProvider.GetDeviceId()) { IsAnonymous = true };

            RaygunRum.Enable(_mauiSettings, _userInfo);
        }
        

        protected override async Task StripAndSend(Exception exception, IList<string> tags, IDictionary userCustomData,
            RaygunIdentifierMessage userInfo)
        {
            foreach (var e in StripWrapperExceptions(exception))
            {
                var msg = await BuildMessage(e, tags, userCustomData, userInfo, customiseMessage: raygunMessage =>
                {
                    raygunMessage.Details.MachineName = DeviceInfo.Current.Name;
                    raygunMessage.Details.Environment = EnvironmentMessageBuilder.BuildEnvironmentMessage();
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
                        raygunMessage.Details.Environment = EnvironmentMessageBuilder.BuildEnvironmentMessage();
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