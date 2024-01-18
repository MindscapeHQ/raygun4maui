#if IOS
using Foundation;
using Raygun4Maui.AppEvents;

using Raygun4Maui.Binding.NetworkMonitor.iOS;

namespace Raygun4Maui.MauiRUM.EventTrackers.Apple;

public class RaygunAppleNativeNetworkObserver
{
    
    private NSObject _networkRequestOccurredObserver;
    
    public void Register()
    {
        // Network monitor events
        _networkRequestOccurredObserver = NSNotificationCenter.DefaultCenter.AddObserver(RaygunNetworkMonitor.RequestOccurredNotificationName, OnNetworkRequestOccurredEvent);
    }
    
    private void OnNetworkRequestOccurredEvent(NSNotification notification)
    {
        
        if (notification?.UserInfo == null)
        {
            return;
        }

        string url    = NSObjectHelper.ToString(notification.UserInfo.ValueForKey(RaygunNetworkMonitor.RequestUrlNotificationKey));
        string method = NSObjectHelper.ToString(notification.UserInfo.ValueForKey(RaygunNetworkMonitor.RequestMethodNotificationKey));
        uint duration = NSObjectHelper.ToUInt(notification.UserInfo.ValueForKey(RaygunNetworkMonitor.RequestDurationNotificationKey));

        RaygunAppEventPublisher.Publish(new NetworkRequestFinished()
        {
            Url = url,
            Method = method,
            Duration = duration
        });
    }

    
}
#endif