using Mindscape.Raygun4Net;

namespace Raygun4Maui;

public interface IRaygunMauiUserProvider : IRaygunUserProvider
{
    void SetUser(RaygunIdentifierMessage user);
}