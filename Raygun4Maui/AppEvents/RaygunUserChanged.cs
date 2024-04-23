using Mindscape.Raygun4Net;

namespace Raygun4Maui.AppEvents;

public class RaygunUserChanged : IRaygunAppEvent
{
    public RaygunIdentifierMessage User;
}