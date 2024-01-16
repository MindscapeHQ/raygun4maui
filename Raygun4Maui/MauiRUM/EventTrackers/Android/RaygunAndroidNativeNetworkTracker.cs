#if ANDROID
using Com.Raygun.Networkmonitorlibrary;

[assembly: Dependency(typeof(RaygunNetworkMonitor))]

namespace Raygun4Maui.MauiRUM.EventTrackers.Android
{
    public class RaygunAndroidNativeNetworkTracker
    {
        public RaygunAndroidNativeNetworkTracker()
        {
            Console.WriteLine("Android Network Tracker Instantiated");
        }

        public void Enable()
        {
            // RaygunLogger.Debug("Enabling network monitor for Android");
            Console.WriteLine("Attempting to enable android network monitor");

            try
            {
                RaygunNetworkMonitor.Enable();
                Console.WriteLine("Success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // RaygunLogger.Error($"Failed to enable the network monitor due to: {e.Message}");
            }
        }
    }
}

#endif