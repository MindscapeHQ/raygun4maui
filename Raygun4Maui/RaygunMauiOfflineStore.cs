using Mindscape.Raygun4Net.Offline;
using Mindscape.Raygun4Net.Storage;

namespace Raygun4Maui;

/// <summary>
/// Represents a crash report store specifically for Raygun in a MAUI application, 
/// utilizing the file system to store crash reports offline.
/// </summary>
public sealed class RaygunMauiOfflineStore : FileSystemCrashReportStore
{
    private const string OfflineStoreId = "raygun.offline-store-id";
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RaygunMauiOfflineStore"/> class.
    /// </summary>
    /// <param name="backgroundSendStrategy">The strategy used to send crash reports in the background.</param>
    /// <param name="maxOfflineFiles">The maximum number of offline files to store. Default is 50.</param>
    public RaygunMauiOfflineStore(IBackgroundSendStrategy backgroundSendStrategy, int maxOfflineFiles = 50)
        : base(backgroundSendStrategy, GetLocalAppDirectory(), maxOfflineFiles)
    {
    }

    private static string GetLocalAppDirectory()
    {
        return Path.Combine(FileSystem.AppDataDirectory, GetOrCreateStoreId());
    }

    private static string GetOrCreateStoreId()
    {
        var storeId = Preferences.Get(OfflineStoreId, null);
        if (string.IsNullOrEmpty(storeId))
        {
            storeId = Guid.NewGuid().ToString("N");
            Preferences.Set(OfflineStoreId, storeId);
        }
        return storeId;
    }
}