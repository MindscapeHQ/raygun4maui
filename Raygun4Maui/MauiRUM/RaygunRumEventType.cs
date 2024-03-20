using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

public enum RaygunRumEventType
{
    None = 0,

    // SimpleJson does not use EnumMember Value's for serialization, these are instead kept for future reference
    [EnumMember(Value = "session_start")]
    SessionStart,

    [EnumMember(Value = "session_end")]
    SessionEnd,

    [EnumMember(Value = "mobile_event_timing")]
    Timing
}