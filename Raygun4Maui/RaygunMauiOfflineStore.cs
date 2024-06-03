using Mindscape.Raygun4Net.Offline;
using Mindscape.Raygun4Net.Storage;

namespace Raygun4Maui;

public sealed class RaygunMauiOfflineStore : FileSystemCrashReportStore
{
    private const string OfflineStoreId = "raygun.offline-store-id";
    
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