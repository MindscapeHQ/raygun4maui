#nullable enable
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.PortableExecutable;

namespace Raygun4Maui;

internal class AssemblyZipEntryReader : AssemblyReader
{
    private readonly ZipArchive _zipArchive;
    private readonly List<string> _supportedAbis;

    public AssemblyZipEntryReader(ZipArchive? zipArchive, List<string> supportedAbis)
    {
        _zipArchive = zipArchive ?? throw new ArgumentNullException(nameof(zipArchive));
        _supportedAbis = supportedAbis;
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

        using var zipStream = zipEntry.Open();
        return CreatePEReader(zipStream.AsReadOnlyMemory());
    }

    private ZipArchiveEntry? FindZipEntryForModule(string moduleName)
    {
        var zipEntry = _zipArchive.GetEntry($"assemblies/{moduleName}");

        if (zipEntry is not null)
            return zipEntry;

        foreach (var abi in _supportedAbis)
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