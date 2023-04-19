
using System.Reflection;
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
            //set the machine name to the device name, this is important for mobile
            e.Message.Details.MachineName = DeviceInfo.Current.Name;
            //set the libary information, without this it will show as a .NET client
            e.Message.Details.Client.Name = "Raygun4Maui";
            e.Message.Details.Client.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(); //reflection might not be a good idea
            e.Message.Details.Client.ClientUrl = "https://github.com/MindscapeHQ/raygun4maui";
        } 
    }
} 
