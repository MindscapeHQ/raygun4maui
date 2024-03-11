using Raygun4Maui.DeviceIdProvider;

namespace Raygun4Maui;

public class RaygunClientInitializeService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var client = services.GetRequiredService<RaygunMauiClient>();
        
        RaygunMauiClient.Attach(client); // Todo: Switch to DI way of getting Raygun client
        // Thoughts: Do we want to use DI as this is easy to use

        var raygunSettings = services.GetRequiredService<Raygun4MauiSettings>();

        if (!raygunSettings.EnableRealUserMonitoring)
        {
            return;
        }
        
        var deviceIdProvider = services.GetRequiredService<IDeviceIdProvider>();

        client.EnableRealUserMonitoring(deviceIdProvider);
    }
}