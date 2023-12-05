using System.Runtime.Serialization;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumTimingInfo
{
    public string Type { get; set; }

    public long Duration { get; set; }
}