using System.Globalization;

namespace Raygun4Maui;

internal class RaygunMauiEnvironmentMessageBuilder
{
    private static readonly object EnvironmentLock = new();

    public string OSVersion { get; init; } = DeviceInfo.Current.VersionString; 
    public string Architecture { get; init; } = NativeDeviceInfo.Architecture(); 

    private string DeviceManufacturer = DeviceInfo.Current.Manufacturer;
    private string Platform = NativeDeviceInfo.Platform();
    private string Model = DeviceInfo.Current.Model;

    private int ProcessorCount = Environment.ProcessorCount;


    private ulong TotalPhysicalMemory = NativeDeviceInfo.TotalPhysicalMemory();
    
    internal RaygunMauiEnvironmentMessage BuildEnvironmentMessage()
    {
        // We create a lock so that during async tasks we do not access device information at the same time
        // this has caused issues in Android
        lock (EnvironmentLock)
        {
            DateTime now = DateTime.Now;

            return new RaygunMauiEnvironmentMessage
            {
                UtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours,
                Locale = CultureInfo.CurrentCulture.DisplayName,
                OSVersion = OSVersion,
                Architecture = Architecture,
                WindowBoundsWidth = DeviceDisplay.MainDisplayInfo.Width,
                WindowBoundsHeight = DeviceDisplay.MainDisplayInfo.Height,
                DeviceManufacturer = DeviceInfo.Current.Manufacturer,
                Platform = Platform,
                Model = Model,
                ProcessorCount = ProcessorCount,
                ResolutionScale = DeviceDisplay.MainDisplayInfo.Density,
                TotalPhysicalMemory = TotalPhysicalMemory,
                AvailablePhysicalMemory = NativeDeviceInfo.AvailablePhysicalMemory(),
                CurrentOrientation = DeviceDisplay.MainDisplayInfo.Orientation.ToString(),
            };
        }
    }

    public static RaygunMauiEnvironmentMessageBuilder Init()
    {
        return new RaygunMauiEnvironmentMessageBuilder();
    }
}