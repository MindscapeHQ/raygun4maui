using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

public enum RaygunRumEventType
{
    None = 0,

    [EnumMember(Value = "session_start")]
    SessionStart,

    [EnumMember(Value = "session_end")]
    SessionEnd,

    [EnumMember(Value = "mobile_event_timing")]
    Timing
}