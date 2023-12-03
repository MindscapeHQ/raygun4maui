using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

[DataContract]
public class RaygunRumMessage
{
    [DataMember]
    public RaygunRumEventInfo[] EventData { get; set; }
}