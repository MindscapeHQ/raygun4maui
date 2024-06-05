#nullable enable
using System.IO.Compression;
using System.Reflection.PortableExecutable;

namespace Raygun4Maui;

internal class AssemblyBlobStoreReader : AssemblyReader
{
    private readonly ZipArchive _zipArchive;

    public AssemblyBlobStoreReader(ZipArchive? zipArchive)
    {
        _zipArchive = zipArchive ?? throw new ArgumentNullException(nameof(zipArchive));
    }

    public override void Dispose()
    {
        _zipArchive.Dispose();
    }

    public override PEReader? TryGetReader(string moduleName)
    {
        throw new NotImplementedException();
    }
}