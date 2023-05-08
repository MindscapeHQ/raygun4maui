
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
        public static RaygunClient Current => _instance;

        internal static void Attach(RaygunClientTest client)
        {
            if (_instance != null)
            {
                throw new Exception("You should only call 'AddRaygun4maui' once in your app.");
            }
          
            _instance = client;

        } 

    }
} 
