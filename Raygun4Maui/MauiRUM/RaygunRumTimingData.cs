using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumTimingData
{
    public string Name { get; set; }

    public RaygunRumTimingInfo Timing { get; set; }
}