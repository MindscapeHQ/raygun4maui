using System.Runtime.Serialization;

namespace Raygun4Maui.MauiRUM;

public class RaygunRumMessage
{
    public RaygunRumEventInfo[] EventData { get; set; }
}