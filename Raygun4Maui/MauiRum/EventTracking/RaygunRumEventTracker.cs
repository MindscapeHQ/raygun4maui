namespace Raygun4Maui.MauiRum.EventTracking
{
    public class RaygunRumEventTracker : IRaygunRumEventTracker
    {
        public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
        {
            LogEventToConsole(eventName, properties);
        }

        private void LogEventToConsole(string eventName, IDictionary<string, string> properties = null)
        {
            System.Diagnostics.Debug.WriteLine($"Event: {eventName}");
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    System.Diagnostics.Debug.WriteLine($"   {prop.Key}: {prop.Value}");
                }
            }
        }
    }

}
