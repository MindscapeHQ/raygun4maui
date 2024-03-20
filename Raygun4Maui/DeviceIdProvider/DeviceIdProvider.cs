namespace Raygun4Maui.DeviceIdProvider;

using Microsoft.Maui.Controls.PlatformConfiguration;

public class DeviceIdProvider : IDeviceIdProvider
{
    public string GetDeviceId()
    { 
        return Preferences.Get(Raygun4MauiExtensions.DeviceIdKey, null);
    }
}