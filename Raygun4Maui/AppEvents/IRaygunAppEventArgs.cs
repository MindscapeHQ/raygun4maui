namespace Raygun4Maui.AppEvents;

public interface IRaygunAppEventArgs
{
  RaygunAppEventType Type { get; }
}