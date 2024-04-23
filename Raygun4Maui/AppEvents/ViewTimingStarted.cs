namespace Raygun4Maui.AppEvents;

public class ViewTimingStarted : IRaygunAppEvent
{
    public string Name { get; set; }

    public long OccurredOn { get; set; }
}