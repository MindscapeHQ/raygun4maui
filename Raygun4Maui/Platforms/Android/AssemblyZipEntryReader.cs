#nullable enable
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.PortableExecutable;

namespace Raygun4Maui;

internal class AssemblyZipEntryReader : AssemblyReader
{
    private readonly ZipArchive _zipArchive;

    public AssemblyZipEntryReader(ZipArchive? zipArchive)
    {
        _zipArchive = zipArchive ?? throw new ArgumentNullException(nameof(zipArchive));
    }

    public override void Dispose()
    {
        _zipArchive.Dispose();
    }

    public override PEReader? TryGetReader(string moduleName)
    {
        // Check the file system first
        if (File.Exists(moduleName))
        {
            var peBytes = File.ReadAllBytes(moduleName).ToImmutableArray();
            return new PEReader(peBytes);
        }

        var zipEntry = FindZipEntryForModule(moduleName);
        if (zipEntry is null)
        {
            Debug.WriteLine("Cannot find module {0}", moduleName);
            return null;
        }

        byte[] rawBytes;

        using (var zipStream = zipEntry.Open())
        using (var memoryStream = new MemoryStream())
        {
            zipStream.CopyTo(memoryStream);
            rawBytes = memoryStream.ToArray();
        }

        // Try to decompress the array from LZ4 as it could be LZ4 compressed
        var actualBytes = TryDecompressLZ4AssemblyBytes(rawBytes, out var decompressedBytes)
            ? decompressedBytes.ToImmutableArray()
            : rawBytes.ToImmutableArray();

        return new PEReader(actualBytes);
    }

    private ZipArchiveEntry? FindZipEntryForModule(string moduleName)
    {
        var zipEntry = _zipArchive.GetEntry($"assemblies/{moduleName}");

        if (zipEntry is not null)
            return zipEntry;

        var supportedAbis = Android.OS.Build.SupportedAbis ?? new List<string>();

        foreach (var abi in supportedAbis)
        {
            zipEntry = _zipArchive.GetEntry($"assemblies/{abi}/{moduleName}");
            if (zipEntry is not null)
            {
                return zipEntry;
            }
        }

        return null;
    }
}