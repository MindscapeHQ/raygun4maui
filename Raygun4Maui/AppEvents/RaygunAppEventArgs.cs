namespace Raygun4Maui.AppEvents;

public class RaygunAppEventArgs : IRaygunAppEventArgs
{
    public RaygunAppEventArgs(RaygunAppEventType type)
    {
        Type = type;
    }

    public RaygunAppEventType Type { get; private set; }
}