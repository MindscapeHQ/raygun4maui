using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Offline;

namespace Raygun4Maui;

/// <summary>
/// Represents a crash report store specifically for Raygun in a MAUI application, 
/// utilizing the file system to store crash reports offline.
/// </summary>
public sealed class RaygunMauiOfflineStore : OfflineStoreBase
{
    private const string OfflineStoreId = "raygun.offline-store-id";
    private const string CacheFileExtension = "rgcrash";

    private readonly string _storageDirectory;
    private readonly int _maxOfflineFiles;
    private readonly ConcurrentDictionary<Guid, string> _cacheLocationMap = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RaygunMauiOfflineStore"/> class.
    /// </summary>
    /// <param name="backgroundSendStrategy">The strategy used to send crash reports in the background.</param>
    /// <param name="maxOfflineFiles">The maximum number of offline files to store. Default is 50.</param>
    public RaygunMauiOfflineStore(IBackgroundSendStrategy backgroundSendStrategy, int maxOfflineFiles = 50)
        : base(backgroundSendStrategy)
    {
        _storageDirectory = GetLocalAppDirectory();
        _maxOfflineFiles = maxOfflineFiles;
    }

    // GetAll, Save and Remove are ported from FileSystemCrashReportStore in Mindscape.Raygun4Net.NetCore,
    // which is not part of Mindscape.Raygun4Net.NetCore.Common

    public override async Task<List<CrashReportStoreEntry>> GetAll(CancellationToken cancellationToken)
    {
        var errorRecords = new List<CrashReportStoreEntry>();

        // The directory is only created on the first Save, so the background send would otherwise throw on every tick until then
        if (!Directory.Exists(_storageDirectory))
        {
            return errorRecords;
        }

        var crashFiles = Directory.GetFiles(_storageDirectory, $"*.{CacheFileExtension}");

        foreach (var crashFile in crashFiles)
        {
            try
            {
                using var fileStream = new FileStream(crashFile, FileMode.Open, FileAccess.Read);
                using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                using var reader = new StreamReader(gzipStream, Encoding.UTF8);

                Trace.WriteLine($"Attempting to load offline crash at {crashFile}");
                var jsonString = await reader.ReadToEndAsync();
                var errorRecord = SimpleJson.DeserializeObject<CrashReportStoreEntry>(jsonString);

                errorRecords.Add(errorRecord);
                _cacheLocationMap[errorRecord.Id] = crashFile;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error deserializing offline crash: {0}", ex.ToString());
                File.Move(crashFile, $"{crashFile}.failed");
            }
        }

        return errorRecords;
    }

    public override async Task<bool> Save(string payload, string apiKey, CancellationToken cancellationToken)
    {
        var cacheEntryId = Guid.NewGuid();
        try
        {
            Directory.CreateDirectory(_storageDirectory);

            var crashFiles = Directory.GetFiles(_storageDirectory, $"*.{CacheFileExtension}");
            if (crashFiles.Length >= _maxOfflineFiles)
            {
                Trace.WriteLine($"Maximum offline files of [{_maxOfflineFiles}] has been reached");
                return false;
            }

            var cacheEntry = new CrashReportStoreEntry
            {
                Id = cacheEntryId,
                ApiKey = apiKey,
                MessagePayload = payload
            };
            var filePath = GetFilePathForCacheEntry(cacheEntryId);
            var jsonContent = SimpleJson.SerializeObject(cacheEntry);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            using var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal);
            using var writer = new StreamWriter(gzipStream, Encoding.UTF8);

            Trace.WriteLine($"Saving crash {cacheEntry.Id} to {filePath}");
            await writer.WriteAsync(jsonContent);
            await writer.FlushAsync();

            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error adding crash [{cacheEntryId}] to store: {ex}");
            return false;
        }
    }

    public override Task<bool> Remove(Guid cacheId, CancellationToken cancellationToken)
    {
        try
        {
            if (_cacheLocationMap.TryGetValue(cacheId, out var filePath))
            {
                var result = RemoveFile(filePath);
                _cacheLocationMap.TryRemove(cacheId, out _);
                return Task.FromResult(result);
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error remove crash [{cacheId}] from store: {ex}");
        }

        return Task.FromResult(false);
    }

    private static bool RemoveFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        File.Delete(filePath);
        return true;
    }

    private string GetFilePathForCacheEntry(Guid cacheId)
    {
        return Path.Combine(_storageDirectory, $"{cacheId:N}.{CacheFileExtension}");
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