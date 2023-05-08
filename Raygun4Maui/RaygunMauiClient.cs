
using System.Reflection;
using Mindscape.Raygun4Net;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Devices;
#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
#endif

namespace Raygun4Maui
{
    public static class RaygunMauiClient
    {
        private static RaygunClientTest _instance;
        
        private static readonly string clientName = "Raygun4Maui";

        private static readonly string clientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static readonly string clientUrl = "https://github.com/MindscapeHQ/raygun4maui";

/*        private static readonly RaygunEnvironmentMessage environmentMessage = new RaygunEnvironmentMessage
        {
            ProcessorCount = System.Environment.ProcessorCount,
            OSVersion = DeviceInfo.Current.VersionString,
            //Cpu = DeviceInfo.Model, // Might not be the exact CPU, but closest we can get
            Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
           *//* TotalVirtualMemory = 0, // Not exactly virtual memory, but total managed heap size
            AvailableVirtualMemory = 0,*//*
            DiskSpaceFree = new List<double>(),
            UtcOffset = DateTimeOffset.Now.Offset.TotalHours,
            Locale = CultureInfo.CurrentCulture.DisplayName,
        };*/

/*        static RaygunMauiClient()
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            environmentMessage.WindowBoundsWidth = displayInfo.Width;
            environmentMessage.WindowBoundsHeight = displayInfo.Height;

        }
*/

        public static RaygunClient Current => _instance;

        internal static void Attach(RaygunClientTest client)
        {
            if (_instance != null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }
          
            _instance = client;
            //client.SendingMessage += OnSendingMessage;
        } 

        private static void OnSendingMessage(object sender, RaygunSendingMessageEventArgs e)
        {
            //set the machine name to the device name, this is important for mobile
            e.Message.Details.MachineName = DeviceInfo.Current.Name;
            //set the libary information, without this it will show as a .NET client
            e.Message.Details.Client.Name = clientName;
            e.Message.Details.Client.Version = clientVersion;
            e.Message.Details.Client.ClientUrl = clientUrl;
            e.Message.Details.Environment.OSVersion = DeviceInfo.Current.VersionString; //IS THIS CORRECT?
            //e.Message.Details.Environment = environmentMessage;

          

        } 
    }
} 
