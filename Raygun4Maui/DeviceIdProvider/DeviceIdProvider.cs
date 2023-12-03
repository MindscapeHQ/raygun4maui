namespace Raygun4Maui.DeviceIdProvider;

using Microsoft.Maui.Controls.PlatformConfiguration;

public class DeviceIdProvider : IDeviceIdProvider
{
    private readonly string _defaultUuid = Guid.NewGuid().ToString();

    public string GetDeviceId()
    { 
        return Preferences.Get(Raygun4MauiExtensions.DeviceIdKey, null);
    }
}