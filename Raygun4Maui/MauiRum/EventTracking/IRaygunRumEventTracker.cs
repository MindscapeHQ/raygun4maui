namespace Raygun4Maui.MauiRum.EventTracking
{
    public interface IRaygunRumEventTracker
    {
        void TrackEvent(string eventName, IDictionary<string, string> properties = null);
    }
}
