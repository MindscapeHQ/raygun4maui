using System.Globalization;
using Mindscape.Raygun4Net;

namespace Raygun4Maui;

internal class RaygunMauiEnvironmentMessageBuilder
{
    private readonly SemaphoreSlim _environmentLock = new(1, 1);
    
    public string OSVersion { get; init; } = DeviceInfo.Current.VersionString; 
    public string Architecture { get; init; } = NativeDeviceInfo.Architecture(); 

    private string DeviceManufacturer = DeviceInfo.Current.Manufacturer;
    private string Platform = NativeDeviceInfo.Platform();
    private string Model = DeviceInfo.Current.Model;

    private int ProcessorCount = Environment.ProcessorCount;


    private ulong TotalPhysicalMemory = NativeDeviceInfo.TotalPhysicalMemory();
    
    internal RaygunMauiEnvironmentMessage BuildEnvironmentMessage()
    {
        // Android has a JNI dereference causing a native crash when many environment messages are built
        // at the same time, so we limit it to one message at a time. Using the InvokeOnMainThread
        // may solve this as well, but we want to be sure that this does not happen
        _environmentLock.Wait(); 
        
        try
        {
            // Cannot get some device specific information on iOS unless you are on the UI thread
            // so we invoke the construction on the main thread
            var environmentMessage = MainThread.InvokeOnMainThreadAsync(() => 
            {
                DateTime now = DateTime.Now;

                return new RaygunMauiEnvironmentMessage
                {
                    UtcOffset = TimeZoneInfo.Local.GetUtcOffset(now).TotalHours,
                    Locale = CultureInfo.CurrentCulture.DisplayName,
                    OSVersion = OSVersion, 
                    Architecture = Architecture,
                    WindowBoundsWidth = DeviceDisplay.MainDisplayInfo.Width,
                    WindowBoundsHeight = DeviceDisplay.MainDisplayInfo.Height,
                    DeviceManufacturer = DeviceManufacturer,
                    Platform = Platform,
                    Model = Model,
                    ProcessorCount = ProcessorCount,
                    ResolutionScale = DeviceDisplay.MainDisplayInfo.Density,
                    TotalPhysicalMemory = TotalPhysicalMemory,
                    AvailablePhysicalMemory = NativeDeviceInfo.AvailablePhysicalMemory(),
                    CurrentOrientation = DeviceDisplay.MainDisplayInfo.Orientation.ToString(),
                };
            }).GetAwaiter().GetResult();

            return environmentMessage;
        }
        finally
        {
            _environmentLock.Release(); // Always release the semaphore
        }
    }

    public static RaygunMauiEnvironmentMessageBuilder Init()
    {
        return new RaygunMauiEnvironmentMessageBuilder();
    }
}
