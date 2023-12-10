using System.Runtime.Serialization;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumTimingInfo
{
    public string type { get; set; }

    public long duration { get; set; }
}