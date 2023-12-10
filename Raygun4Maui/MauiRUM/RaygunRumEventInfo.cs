using System.Runtime.Serialization;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumEventInfo
{
    public RaygunRumEventInfo()
    {
    }

    public string sessionId { get; set; }

    public DateTime timestamp { get; set; }

    public string type { get; set; }

    public RaygunIdentifierMessage user { get; set; }

    public string Version { get; set; }

    public string os { get; set; }

    public string osVersion { get; set; }

    public string platform { get; set; }

    public string data { get; set; }
}