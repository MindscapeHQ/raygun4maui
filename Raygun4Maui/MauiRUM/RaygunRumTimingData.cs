using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

[DataContract]
public class RaygunRumTimingData
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public RaygunRumTimingInfo Timing { get; set; }
}