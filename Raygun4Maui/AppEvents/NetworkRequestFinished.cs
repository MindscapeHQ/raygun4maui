namespace Raygun4Maui.AppEvents;

public class NetworkRequestFinished : IRaygunAppEvent
{
    public string Url { get; set; }
    
    public string Method { get; set; }
    
    public long Duration { get; set; }
}