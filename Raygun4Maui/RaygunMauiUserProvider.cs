using Mindscape.Raygun4Net;
using Raygun4Maui.MauiRUM.EventTrackers;

namespace Raygun4Maui;

public abstract class RaygunMauiUserProvider : IRaygunUserProvider
{

    public void HandleUserChange()
    {
        RaygunSessionTracker.CurrentUser = GetUser();
    }
    
    public RaygunIdentifierMessage GetUser()
    {
        throw new NotImplementedException();
    }
}