using System.Buffers;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using K4os.Compression.LZ4;

namespace Raygun4Maui;

internal abstract class AssemblyReader : IAssemblyReader
{
    protected bool TryDecompressLZ4AssemblyBytes(byte[] assemblyBytes, out ReadOnlySpan<byte> decompressedBytes)
    {
        var estimatedSize = LZ4Codec.MaximumOutputSize(assemblyBytes.Length);
        var buffer = ArrayPool<byte>.Shared.Rent(estimatedSize);

        try
        {
            var actualDecompressedSize = LZ4Codec.Decode(assemblyBytes, buffer);
            decompressedBytes = new ReadOnlySpan<byte>(buffer, 0, actualDecompressedSize);

            return true;
        }
        catch
        {
            decompressedBytes = ReadOnlySpan<byte>.Empty;
        }
        finally
        {
            // Return the buffer to the pool
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return false;
    }

    /// <summary>
    /// Factory method to create the correct assembly reader for the current application
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public static IAssemblyReader Create(string moduleName)
    {
        var apkPath = Android.App.Application.Context.ApplicationInfo?.SourceDir;

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
        var zipArchive = ZipFile.Open(apkPath, ZipArchiveMode.Read);

        if (zipArchive.GetEntry("assemblies/assemblies.manifest") != null)
        {
            return new AssemblyBlobStoreReader(zipArchive);
        }

        return new AssemblyZipEntryReader(zipArchive);
    }

    private static bool IsAndroidArchive(string filePath)
    {
        return filePath.EndsWith(".aab", StringComparison.OrdinalIgnoreCase) ||
               filePath.EndsWith(".apk", StringComparison.OrdinalIgnoreCase) ||
               filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
    }

    public virtual void Dispose()
    {
    }

    public abstract PEReader TryGetReader(string moduleName);
}