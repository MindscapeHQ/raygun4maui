using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM.EventTypes;

public enum RaygunRumEventTimingType
{
    // SimpleJson does not use EnumMember Value's for serialization, these are instead kept for future reference
    [EnumMember(Value = "p")]
    ViewLoaded,

    [EnumMember(Value = "n")]
    NetworkCall,
    
    [EnumMember(Value = "t")]
    CustomTiming
}