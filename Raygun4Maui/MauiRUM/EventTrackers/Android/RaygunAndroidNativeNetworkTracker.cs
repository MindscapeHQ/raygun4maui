#if ANDROID
using Com.Raygun.Networkmonitorlibrary;

[assembly: Dependency(typeof(RaygunNetworkMonitor))]

namespace Raygun4Maui.MauiRUM.EventTrackers.Android
{
    public class RaygunAndroidNativeNetworkTracker
    {
        public RaygunAndroidNativeNetworkTracker()
        {
        }

        public void Enable()
        {
            // RaygunLogger.Debug("Enabling network monitor for Android");

            try
            {
                RaygunNetworkMonitor.Enable();
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