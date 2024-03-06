using System.Globalization;
using Mindscape.Raygun4Net;

namespace Raygun4Maui;

internal class RaygunMauiEnvironmentMessageBuilder
{
    public string OSVersion { get; init; } = DeviceInfo.Current.VersionString;
    public string Architecture { get; init; } = NativeDeviceInfo.Architecture();

    private string DeviceManufacturer = DeviceInfo.Current.Manufacturer;
    private string Platform = NativeDeviceInfo.Platform();
    private string Model = DeviceInfo.Current.Model;

    private int ProcessorCount = Environment.ProcessorCount;

    private double UtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;


    private ulong TotalPhysicalMemory = NativeDeviceInfo.TotalPhysicalMemory();


    private string Locale = CultureInfo.CurrentCulture.DisplayName;

    private double? WindowBoundsWidth = null;

    private double? WindowBoundsHeight = null;

    private double? ResolutionScale = null;

    private string CurrentOrientation = null;

    internal RaygunMauiEnvironmentMessage BuildEnvironmentMessage()
    {
        return new RaygunMauiEnvironmentMessage
        {
            UtcOffset = UtcOffset,
            Locale = Locale,
            OSVersion = OSVersion,
            Architecture = Architecture,
            WindowBoundsWidth = WindowBoundsWidth ?? 0,
            WindowBoundsHeight = WindowBoundsHeight ?? 0,
            DeviceManufacturer = DeviceManufacturer,
            Platform = Platform,
            Model = Model,
            ProcessorCount = ProcessorCount,
            ResolutionScale = ResolutionScale ?? 0,
            TotalPhysicalMemory = TotalPhysicalMemory,
            AvailablePhysicalMemory =
                NativeDeviceInfo.AvailablePhysicalMemory(), // Only possible issue for concurrent access
            CurrentOrientation = CurrentOrientation,
        };
    }

    private void UpdateDisplayInfo(object sender, DisplayInfoChangedEventArgs args)
    {
        // We assume the update will come from the UI thread

        WindowBoundsWidth = args.DisplayInfo.Width;
        WindowBoundsHeight = args.DisplayInfo.Height;
        ResolutionScale = args.DisplayInfo.Density;
        CurrentOrientation = args.DisplayInfo.Orientation.ToString();
    }

    public RaygunMauiEnvironmentMessageBuilder()
    {
        DeviceDisplay.MainDisplayInfoChanged += UpdateDisplayInfo;

        // Instantiated here as RaygunMauiEnvironmentMessageBuilder is lazily initialised so we can be sure the DeviceDisplay has an instance
        MainThread.InvokeOnMainThreadAsync(() => UpdateDisplayInfo(this, new DisplayInfoChangedEventArgs(DeviceDisplay.MainDisplayInfo)));
    }
}