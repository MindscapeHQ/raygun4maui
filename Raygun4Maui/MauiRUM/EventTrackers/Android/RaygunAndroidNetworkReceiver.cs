#if ANDROID
using Android.Content;
using Raygun4Maui.AppEvents;

namespace Raygun4Maui.MauiRUM.EventTrackers.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class RaygunAndroidNetworkReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        string url = intent.GetStringExtra("url");
        string method = intent.GetStringExtra("method");
        long duration = intent.GetLongExtra("duration", 0);

        if (url != null && duration > 0 && !url.Contains("raygun"))
        {
            RaygunAppEventPublisher.Publish(new NetworkRequestFinished()
            {
                Url = url,
                Method = method,
                Duration = duration
            });
        }
    }
}
#endif