#nullable enable
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace Raygun4Maui;

internal static class AndroidUtilities
{
    /// <summary>
    /// Factory method to create the correct assembly reader for the current application
    /// </summary>
    /// <returns></returns>
    public static IAssemblyReader? CreateAssemblyReader()
    {
        var apkPath = Android.App.Application.Context.ApplicationInfo?.SourceDir;
        var supportedAbis = new List<string>();

        if (Android.OS.Build.SupportedAbis != null)
        {
            supportedAbis.AddRange(Android.OS.Build.SupportedAbis);
        }

        if (!File.Exists(apkPath))
        {
            // No apk, so return nothing
            return null;
        }

        if (!IsAndroidArchive(apkPath))
        {
            // Not a valid android archive so nothing to return
            return null;
        }
        
        // Open the apk file, and see if it has a manifest, if it does,
        // we are using the new assembly store method,
        // else it's just a normal zip with assemblies as archive entries
        using var zipArchive = ZipFile.Open(apkPath, ZipArchiveMode.Read);

        if (zipArchive.GetEntry("assemblies/assemblies.manifest") != null)
        {
            return new AssemblyBlobStoreReader(zipArchive, supportedAbis);
        }

        return new AssemblyZipEntryReader(zipArchive, supportedAbis);
    }

    public static bool IsAndroidArchive(string filePath)
    {
        return filePath.EndsWith(".aab", StringComparison.OrdinalIgnoreCase) ||
               filePath.EndsWith(".apk", StringComparison.OrdinalIgnoreCase) ||
               filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
    }

    public static ReadOnlyMemory<byte> AsReadOnlyMemory(this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return new ReadOnlyMemory<byte>(memoryStream.ToArray());
    }

    public static byte[] ToArray(this ReadOnlyMemory<byte> memory)
    {
        if (!MemoryMarshal.TryGetArray(memory, out var segment))
        {
            throw new InvalidOperationException("Could not get array segment from ReadOnlyMemory.");
        }

        return segment.Array!;
    }

    public static BinaryReader GetBinaryReader(this ReadOnlyMemory<byte> memory, Encoding? encoding = null)
    {
        return new BinaryReader(memory.AsStream(), encoding ?? Encoding.UTF8, false);
    }


    public static MemoryStream AsStream(this ReadOnlyMemory<byte> memory)
    {
        return new MemoryStream(memory.ToArray(), writable: false);
    }
}