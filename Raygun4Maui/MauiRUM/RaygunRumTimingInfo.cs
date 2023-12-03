using System.Runtime.Serialization;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM;

[DataContract]
public class RaygunRumTimingInfo
{
    [DataMember]
    public RaygunRumEventTimingType Type { get; set; }

    [DataMember]
    public long Duration { get; set; }
}