
using System.Reflection;
using Mindscape.Raygun4Net;

namespace Raygun4Maui
{
    public static class RaygunMauiClient
    {
        private static RaygunClient _instance;
        
        private static readonly string clientName = "Raygun4Maui";

        private static readonly string clientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static readonly string clientUrl = "https://github.com/MindscapeHQ/raygun4maui";

        public static RaygunClient Current => _instance;

        internal static void Attach(RaygunClient client)
        {
            if (_instance != null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }
          
            _instance = client;
            client.SendingMessage += OnSendingMessage;
        } 

        private static void OnSendingMessage(object sender, RaygunSendingMessageEventArgs e)
        {
            //set the machine name to the device name, this is important for mobile
            e.Message.Details.MachineName = DeviceInfo.Current.Name;
            //set the libary information, without this it will show as a .NET client
            e.Message.Details.Client.Name = clientName;
            e.Message.Details.Client.Version = clientVersion;
            e.Message.Details.Client.ClientUrl = clientUrl;
        } 
    }
} 
