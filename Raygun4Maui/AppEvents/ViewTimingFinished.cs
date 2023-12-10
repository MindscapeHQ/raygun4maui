namespace Raygun4Maui.AppEvents;

public class ViewTimingFinished : IRaygunAppEvent
{
    public string Name { get; set; }

    public long OccurredOn { get; set; }
}