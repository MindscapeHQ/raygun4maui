namespace Raygun4Maui.MauiRUM.EventTypes;

public sealed class RaygunTimingEventArgs : EventArgs
{
    public RaygunTimingEventArgs()
    {
    }

    public RaygunRumEventTimingType Type { get; set; }

    public string Key { get; set; }

    public long Milliseconds { get; set; }
}