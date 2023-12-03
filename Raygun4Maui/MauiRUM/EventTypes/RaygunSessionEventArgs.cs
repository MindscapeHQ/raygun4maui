using Mindscape.Raygun4Net;

namespace Raygun4Maui.MauiRUM.EventTypes;

public sealed class RaygunSessionEventArgs : EventArgs
{
  public RaygunSessionEventArgs(string sessionId, RaygunIdentifierMessage user)
  {
    SessionId = sessionId;
    User = user;
  }

  public string SessionId { get; private set; }
  public RaygunIdentifierMessage User { get; private set; }
}