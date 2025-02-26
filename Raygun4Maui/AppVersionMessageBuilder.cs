using Mindscape.Raygun4Net;

namespace Raygun4Maui;

public class AppVersionMessageBuilder : IMessageBuilder
{
    private readonly Raygun4MauiSettings _settings;
    
    public AppVersionMessageBuilder(Raygun4MauiSettings settings)
    {
        Console.WriteLine("Activating app version message builder");
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    
    public Task<RaygunMessage> Apply(RaygunMessage message, Exception exception)
    {
        Console.WriteLine("Entering app version message builder");
        if (!_settings.UseAppVersion) return Task.FromResult(message);
        
        Console.WriteLine("Changing the message details for App Version");
        
        message.Details.Version = $"{AppInfo.VersionString}.{AppInfo.BuildString}";

        return Task.FromResult(message);
    }
}