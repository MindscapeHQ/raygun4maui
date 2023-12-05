using System.Runtime.Serialization;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumEventInfo
{
    public RaygunRumEventInfo()
    {
    }

    public string SessionId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Type { get; set; }

    public RaygunIdentifierMessage User { get; set; }

    public string Version { get; set; }

    public string Os { get; set; }

    public string OsVersion { get; set; }

    public string Platform { get; set; }

    public string Data { get; set; }
}