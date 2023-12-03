using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM.EventTypes;

public enum RaygunRumEventTimingType
{
    [EnumMember(Value = "p")]
    ViewLoaded,

    [EnumMember(Value = "n")]
    NetworkCall,
    
    [EnumMember(Value = "t")]
    CustomTiming
}