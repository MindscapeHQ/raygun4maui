#if IOS || MACCATALYST
using Raygun4Maui.Binding.NetworkMonitor.iOS;

namespace Raygun4Maui.MauiRUM.EventTrackers.Apple;

public class RaygunAppleNativeNetworkMonitor
{
    public RaygunAppleNativeNetworkMonitor(){}

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
#endif
