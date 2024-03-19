using Raygun4Maui.DeviceIdProvider;

namespace Raygun4Maui;

public class RaygunClientInitializeService : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var client = services.GetRequiredService<RaygunMauiClient>();
        
        RaygunMauiClient.Attach(client);

        var raygunSettings = services.GetRequiredService<Raygun4MauiSettings>();

        if (!raygunSettings.EnableRealUserMonitoring)
        {
            return;
        }
        
        var deviceIdProvider = services.GetRequiredService<IDeviceIdProvider>();

        client.EnableRealUserMonitoring(deviceIdProvider);
    }
}