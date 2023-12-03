using System.Runtime.Serialization;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.MauiRUM;

[DataContract]
public class RaygunRumEventInfo
{
    public RaygunRumEventInfo()
    {
    }

    [DataMember]
    public string SessionId { get; set; }

    [DataMember]
    public DateTime Timestamp { get; set; }

    [DataMember]
    public RaygunRumEventType Type { get; set; }

    [DataMember]
    public RaygunIdentifierMessage User { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public string Os { get; set; }

    [DataMember]
    public string OsVersion { get; set; }

    [DataMember]
    public string Platform { get; set; }

    [DataMember]
    public string Data { get; set; }
}