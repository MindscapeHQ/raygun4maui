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
            try
            {
                RaygunNetworkMonitor.Enable();
            }
            catch (Exception ignore)
            {
            }
        }
    }
}

#endif