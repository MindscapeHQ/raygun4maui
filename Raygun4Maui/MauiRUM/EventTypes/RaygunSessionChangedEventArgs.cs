using Mindscape.Raygun4Net;

namespace Raygun4Maui.MauiRUM.EventTypes;

public sealed class RaygunSessionChangedEventArgs : EventArgs
{
    public RaygunSessionChangedEventArgs(string oldSessionId, string newSessionId, RaygunIdentifierMessage oldUser, RaygunIdentifierMessage newUser)
    {
        OldSessionId = oldSessionId;
        NewSessionId = newSessionId;
        OldUser = oldUser;
        NewUser = newUser;
    }

    public string OldSessionId { get; private set; }
    public string NewSessionId { get; private set; }

    public RaygunIdentifierMessage OldUser { get; private set; }
    public RaygunIdentifierMessage NewUser { get; private set; }
}