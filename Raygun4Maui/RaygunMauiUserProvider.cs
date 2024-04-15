using Mindscape.Raygun4Net;
using Raygun4Maui.AppEvents;

namespace Raygun4Maui;

public class RaygunMauiUserProvider : IRaygunMauiUserProvider
{
    private RaygunIdentifierMessage _user = null!;

    public RaygunIdentifierMessage GetUser()
    {
        return _user;
    }

    public void SetUser(RaygunIdentifierMessage user)
    {
        _user = user;

        RaygunAppEventPublisher.Publish(new RaygunUserChanged
        {
            User = _user
        });
    }
}