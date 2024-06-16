#nullable enable
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Reflection.PortableExecutable;
using K4os.Compression.LZ4;

namespace Raygun4Maui;

internal abstract class AssemblyReader : IAssemblyReader
{
    private const uint CompressedDataMagic = 0x5A4C4158; // 'XALZ', little-endian

    public virtual void Dispose()
    {
    }

    public abstract PEReader? TryGetReader(string moduleName);

    protected PEReader CreatePEReader(ReadOnlyMemory<byte> rawBytes)
    {
        // Try to decompress the array from LZ4 as it could be LZ4 compressed
        var actualBytes = TryDecompressLZ4AssemblyBytes(rawBytes, out var decompressedBytes)
            ? decompressedBytes
            : rawBytes;

        return new PEReader(actualBytes.Span.ToImmutableArray());
    }

    // Decompression reference : https://github.com/xamarin/xamarin-android/blob/c92702619f5fabcff0ed88e09160baf9edd70f41/tools/decompress-assemblies/main.cs
    private bool TryDecompressLZ4AssemblyBytes(ReadOnlyMemory<byte> encodedAssembly, out ReadOnlyMemory<byte> decompressedBytes)
    {
        var magicHeader = BinaryPrimitives.ReadUInt32LittleEndian(encodedAssembly.Span[..4]);
        var decompressedLength = BinaryPrimitives.ReadInt32LittleEndian(encodedAssembly.Span[8..12]);

        if (magicHeader != CompressedDataMagic)
        {
            decompressedBytes = ReadOnlyMemory<byte>.Empty;
            return false;
        }

        // Skip the header and move to the compressed bytes
        var assemblyBytes = encodedAssembly.Slice(12);
        var decompressedArray = new byte[decompressedLength];

        try
        {
            var actualDecompressedSize = LZ4Codec.Decode(assemblyBytes.Span, decompressedArray);

            if (actualDecompressedSize != decompressedLength)
            {
                throw new Exception($"Could not decompress bytes. Lengths do not match, expected {decompressedLength} but got {actualDecompressedSize}");
            }

            decompressedBytes = new ReadOnlyMemory<byte>(decompressedArray);
            return true;
        }
        catch
        {
            decompressedBytes = ReadOnlyMemory<byte>.Empty;
        }

        return false;
    }
}