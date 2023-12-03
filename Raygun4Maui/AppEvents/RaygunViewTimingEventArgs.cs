namespace Raygun4Maui.AppEvents;

public class RaygunViewTimingEventArgs : IRaygunAppEventArgs
{
    public RaygunViewTimingEventArgs()
    {
    }

    public RaygunAppEventType Type { get; set; }

    public string Name { get; set; }

    public long OccurredOn { get; set; }
}