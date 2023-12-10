using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumTimingData
{
    public string name { get; set; }

    public RaygunRumTimingInfo timing { get; set; }
}