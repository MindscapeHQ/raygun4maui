
using Mindscape.Raygun4Net;

namespace Raygun4Maui
{
    public static class RaygunMauiClient
    {
        private static RaygunClient _instance;

        public static RaygunClient Current => _instance;

        public static void Attach(RaygunClient client)
        {
            if (_instance != null)
            {
                //Throwing an exception is probably not the best way to handle this?
                throw new Exception("RaygunMauiClient already initialized");
            }
          
            _instance = client;
            client.SendingMessage += OnSendingMessage;
        } 

        private static void OnSendingMessage(object sender, RaygunSendingMessageEventArgs e)
        {
            e.Message.Details.MachineName = DeviceInfo.Current.Name;
        } 
    }
} 
