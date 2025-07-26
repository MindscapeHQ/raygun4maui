namespace Raygun4Maui.DeviceIdProvider;

public class DeviceIdProvider : IDeviceIdProvider
{
    public string GetDeviceId()
    { 
        return Preferences.Get(Raygun4MauiExtensions.DeviceIdKey, null);
    }
}