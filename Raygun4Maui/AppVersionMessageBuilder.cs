using Mindscape.Raygun4Net;

namespace Raygun4Maui;

public class AppVersionMessageBuilder : IMessageBuilder
{
    private readonly Raygun4MauiSettings _settings;
    
    public AppVersionMessageBuilder(Raygun4MauiSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    
    public Task<RaygunMessage> Apply(RaygunMessage message, Exception exception)
    {
        if (!_settings.UseAppVersion) return Task.FromResult(message);
        
        message.Details.Version = $"{AppInfo.VersionString}.{AppInfo.BuildString}";

        return Task.FromResult(message);
    }
}