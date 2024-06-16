#nullable enable
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Raygun4Maui;

internal class AssemblyBlobStoreReader : AssemblyReader
{
    private readonly AssemblyStoreExplorer _assemblyStoreExplorer;

    public AssemblyBlobStoreReader(ZipArchive zipArchive, List<string> supportedAbis)
    {
        ArgumentNullException.ThrowIfNull(zipArchive);

        _assemblyStoreExplorer = new AssemblyStoreExplorer(zipArchive, supportedAbis);
    }

    public override PEReader? TryGetReader(string moduleName)
    {
        var assembly = TryFindAssembly(moduleName);

        if (assembly is null)
        {
            return null;
        }

        var bytes = assembly.GetImageData();

        return CreatePEReader(bytes);
    }

    private AssemblyStoreAssembly? TryFindAssembly(string name)
    {
        if (_assemblyStoreExplorer.AssembliesByName.TryGetValue(name, out var assembly))
        {
            return assembly;
        }

        // Try get the assembly without the module extension
        var pathLessExtension = Path.GetFileNameWithoutExtension(name);
        if (_assemblyStoreExplorer.AssembliesByName.TryGetValue(pathLessExtension, out assembly))
        {
            return assembly;
        }

        return null;
    }

    public override void Dispose()
    {
    }


    /// <summary>
    /// <remarks>https://github.com/xamarin/xamarin-android/blob/c92702619f5fabcff0ed88e09160baf9edd70f41/tools/assembly-store-reader/AssemblyStoreExplorer.cs</remarks>
    /// </summary>
    private class AssemblyStoreExplorer
    {
        private readonly AssemblyStoreManifestReader _manifest;
        private AssemblyStore? _indexStore;

        public SortedDictionary<string, AssemblyStoreAssembly> AssembliesByName { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<uint, AssemblyStoreAssembly> AssembliesByHash32 { get; } = new();

        public Dictionary<ulong, AssemblyStoreAssembly> AssembliesByHash64 { get; } = new();

        public SortedDictionary<uint, List<AssemblyStore>> Stores { get; } = new();

        public AssemblyStoreExplorer(ZipArchive zipArchive, List<string> supportedAbis)
        {
            using var manifestStream = zipArchive.GetEntry("assemblies/assemblies.manifest")!.Open();
            _manifest = new AssemblyStoreManifestReader(manifestStream);

            TryAddStore(zipArchive, null);
            foreach (var abi in supportedAbis)
            {
                if (!string.IsNullOrEmpty(abi))
                {
                    TryAddStore(zipArchive, abi);
                }
            }

            ProcessStores();
        }

        private void TryAddStore(ZipArchive zipArchive, string? abi)
        {
            var storeLocation = abi is null
                ? "assemblies/assemblies.blob"
                : $"assemblies/assemblies.{abi}.blob";

            if (zipArchive.GetEntry(storeLocation) is { } zipEntry)
            {
                using var entry = zipEntry.Open();
                var store = new AssemblyStore(entry.AsReadOnlyMemory(), abi);

                if (store.HasGlobalIndex)
                {
                    _indexStore = store;
                }

                if (!Stores.TryGetValue(store.StoreId, out var storeList))
                {
                    storeList = new List<AssemblyStore>();
                    Stores.Add(store.StoreId, storeList);
                }

                storeList.Add(store);
            }
        }

        private void ProcessStores()
        {
            if (Stores.Count == 0 || _indexStore == null)
            {
                return;
            }

            ProcessIndex(_indexStore.GlobalIndex32, (he, assembly) =>
            {
                assembly.Hash32 = (uint)he.Hash;
                assembly.RuntimeIndex = he.MappingIndex;

                if (_manifest.EntriesByHash32.TryGetValue(assembly.Hash32, out var me))
                {
                    assembly.Name = me.Name;
                }

                if (!AssembliesByHash32.ContainsKey(assembly.Hash32))
                {
                    AssembliesByHash32.Add(assembly.Hash32, assembly);
                }
            });

            ProcessIndex(_indexStore.GlobalIndex64, (he, assembly) =>
            {
                assembly.Hash64 = he.Hash;
                if (assembly.RuntimeIndex != he.MappingIndex)
                {
                    Debug.WriteLine($"assembly with hashes 0x{assembly.Hash32} and 0x{assembly.Hash64} has a different 32-bit runtime index ({assembly.RuntimeIndex}) than the 64-bit runtime index({he.MappingIndex})");
                }

                if (_manifest.EntriesByHash64.TryGetValue(assembly.Hash64, out var me))
                {
                    if (string.IsNullOrEmpty(assembly.Name))
                    {
                        Debug.WriteLine($"32-bit hash 0x{assembly.Hash32:x} did not match any assembly name in the manifest");
                        assembly.Name = me.Name;
                        if (string.IsNullOrEmpty(assembly.Name))
                        {
                            Debug.WriteLine($"64-bit hash 0x{assembly.Hash64:x} did not match any assembly name in the manifest");
                        }
                    }
                    else if (!string.Equals(assembly.Name, me.Name, StringComparison.Ordinal))
                    {
                        Debug.WriteLine($"32-bit hash 0x{assembly.Hash32:x} maps to assembly name '{assembly.Name}', however 64-bit hash 0x{assembly.Hash64:x} for the same entry matches assembly name '{me.Name}'");
                    }
                }

                if (!AssembliesByHash64.ContainsKey(assembly.Hash64))
                {
                    AssembliesByHash64.Add(assembly.Hash64, assembly);
                }
            });

            foreach (var kvp in Stores)
            {
                var list = kvp.Value;
                if (list.Count < 2)
                {
                    continue;
                }

                var template = list[0];
                for (var i = 1; i < list.Count; i++)
                {
                    var other = list[i];
                    if (!template.HasIdenticalContent(other))
                    {
                        throw new Exception($"Store ID {template.StoreId} for architecture {other.Arch} is not identical to other stores with the same ID");
                    }
                }
            }

            return;

            void ProcessIndex(List<AssemblyStoreHashEntry> index, Action<AssemblyStoreHashEntry, AssemblyStoreAssembly> assemblyHandler)
            {
                foreach (var he in index)
                {
                    if (!Stores.TryGetValue(he.StoreId, out var storeList))
                    {
                        continue;
                    }

                    foreach (var store in storeList)
                    {
                        if (he.LocalStoreIndex >= (uint)store.Assemblies.Count)
                        {
                            continue;
                        }

                        var assembly = store.Assemblies[(int)he.LocalStoreIndex];
                        assemblyHandler(he, assembly);

                        AssembliesByName.TryAdd(assembly.Name, assembly);
                    }
                }
            }
        }
    }

    // https://github.com/xamarin/xamarin-android/blob/main/tools/assembly-store-reader/AssemblyStoreManifestReader.cs
    private class AssemblyStoreManifestReader
    {
        public List<AssemblyStoreManifestEntry> Entries { get; } = new();

        public Dictionary<uint, AssemblyStoreManifestEntry> EntriesByHash32 { get; } = new();

        public Dictionary<ulong, AssemblyStoreManifestEntry> EntriesByHash64 { get; } = new();

        public AssemblyStoreManifestReader(Stream manifest)
        {
            using var sr = new StreamReader(manifest, Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            ReadManifest(sr);
        }

        private void ReadManifest(StreamReader reader)
        {
            // First line is ignored, it contains headers
            reader.ReadLine();

            // Each subsequent line consists of fields separated with any number of spaces (for the pleasure of a human being reading the manifest)
            while (!reader.EndOfStream)
            {
                var fields = reader.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (fields == null)
                {
                    continue;
                }

                var entry = new AssemblyStoreManifestEntry(fields);
                Entries.Add(entry);
                if (entry.Hash32 != 0)
                {
                    EntriesByHash32.Add(entry.Hash32, entry);
                }

                if (entry.Hash64 != 0)
                {
                    EntriesByHash64.Add(entry.Hash64, entry);
                }
            }
        }
    }

    /// <summary>
    /// <remarks>https://github.com/xamarin/xamarin-android/blob/c92702619f5fabcff0ed88e09160baf9edd70f41/tools/assembly-store-reader/AssemblyStoreHashEntry.cs</remarks>
    /// </summary>
    private class AssemblyStoreHashEntry
    {
        public bool Is32Bit { get; }
        public ulong Hash { get; }
        public uint MappingIndex { get; }
        public uint LocalStoreIndex { get; }
        public uint StoreId { get; }

        internal AssemblyStoreHashEntry(BinaryReader reader, bool is32Bit)
        {
            Is32Bit = is32Bit;

            Hash = reader.ReadUInt64();
            MappingIndex = reader.ReadUInt32();
            LocalStoreIndex = reader.ReadUInt32();
            StoreId = reader.ReadUInt32();
        }
    }

    /// <summary>
    /// <remarks>https://github.com/xamarin/xamarin-android/blob/c92702619f5fabcff0ed88e09160baf9edd70f41/tools/assembly-store-reader/AssemblyStoreManifestEntry.cs</remarks>
    /// </summary>
    private class AssemblyStoreManifestEntry
    {
        // Fields are:
        //  Hash 32 | Hash 64 | Store ID | Store idx | Name
        private const int NumberOfFields = 5;
        private const int Hash32FieldIndex = 0;
        private const int Hash64FieldIndex = 1;
        private const int StoreIdFieldIndex = 2;
        private const int StoreIndexFieldIndex = 3;
        private const int NameFieldIndex = 4;

        public uint Hash32 { get; }
        public ulong Hash64 { get; }
        public uint StoreId { get; }
        public uint IndexInStore { get; }
        public string Name { get; }

        public AssemblyStoreManifestEntry(string[] fields)
        {
            if (fields.Length != NumberOfFields)
            {
                throw new ArgumentOutOfRangeException(nameof(fields), "Invalid number of fields");
            }

            Hash32 = GetUInt32(fields[Hash32FieldIndex]);
            Hash64 = GetUInt64(fields[Hash64FieldIndex]);
            StoreId = GetUInt32(fields[StoreIdFieldIndex]);
            IndexInStore = GetUInt32(fields[StoreIndexFieldIndex]);
            Name = fields[NameFieldIndex].Trim();
        }

        private static uint GetUInt32(string value)
        {
            return uint.TryParse(PrepHexValue(value), NumberStyles.HexNumber, null, out uint hash) ? hash : 0;
        }

        private static ulong GetUInt64(string value)
        {
            return ulong.TryParse(PrepHexValue(value), NumberStyles.HexNumber, null, out ulong hash) ? hash : 0;
        }

        private static string PrepHexValue(string value)
        {
            return value.StartsWith("0x", StringComparison.Ordinal) ? value[2..] : value;
        }
    }

    // https://github.com/xamarin/xamarin-android/blob/main/tools/assembly-store-reader/AssemblyStoreAssembly.cs
    private class AssemblyStoreAssembly
    {
        private readonly Func<uint, uint, ReadOnlyMemory<byte>> _storeDataFunc;
        public uint DataOffset { get; }
        public uint DataSize { get; }
        public uint DebugDataOffset { get; }
        public uint DebugDataSize { get; }
        public uint ConfigDataOffset { get; }
        public uint ConfigDataSize { get; }

        public uint Hash32 { get; set; }
        public ulong Hash64 { get; set; }
        public string Name { get; set; } = String.Empty;
        public uint RuntimeIndex { get; set; }

        public AssemblyStoreAssembly(BinaryReader reader, Func<uint, uint, ReadOnlyMemory<byte>> storeDataFunc)
        {
            _storeDataFunc = storeDataFunc;
            DataOffset = reader.ReadUInt32();
            DataSize = reader.ReadUInt32();
            DebugDataOffset = reader.ReadUInt32();
            DebugDataSize = reader.ReadUInt32();
            ConfigDataOffset = reader.ReadUInt32();
            ConfigDataSize = reader.ReadUInt32();
        }

        public ReadOnlyMemory<byte> GetImageData()
        {
            return _storeDataFunc(DataOffset, DataSize);
        }
    }

    // https://github.com/xamarin/xamarin-android/blob/main/tools/assembly-store-reader/AssemblyStoreReader.cs
    private sealed class AssemblyStore
    {
        // These two constants must be identical to the native ones in src/monodroid/jni/xamarin-app.hh
        private const uint AssemblyStoreMagic = 0x41424158; // 'XABA', little-endian
        private const uint AssemblyStoreFormatVersion = 1;

        private readonly ReadOnlyMemory<byte> _storeMemory;

        public uint Version { get; private set; }
        public uint LocalEntryCount { get; private set; }
        public uint GlobalEntryCount { get; private set; }
        public uint StoreId { get; private set; }

        public bool HasGlobalIndex => StoreId == 0;
        public List<AssemblyStoreAssembly> Assemblies { get; } = new();
        public List<AssemblyStoreHashEntry> GlobalIndex32 { get; } = new();
        public List<AssemblyStoreHashEntry> GlobalIndex64 { get; } = new();

        public string Arch { get; }

        public AssemblyStore(ReadOnlyMemory<byte> storeMemory, string? arch)
        {
            _storeMemory = storeMemory;
            Arch = arch ?? string.Empty;

            // Need to use a reader here, because the reader advances through the memory
            // and the structure is based off the current position
            using var reader = _storeMemory.GetBinaryReader();
            ReadHeader(reader);
            LoadAssemblies(reader);

            if (HasGlobalIndex)
            {
                ReadGlobalIndex(reader);
            }
        }

        private void ReadGlobalIndex(BinaryReader reader)
        {
            // Read 32 bit index
            for (uint i = 0; i < GlobalEntryCount; i++)
            {
                GlobalIndex32.Add(new AssemblyStoreHashEntry(reader, true));
            }

            // Read 64 bit index
            for (uint i = 0; i < GlobalEntryCount; i++)
            {
                GlobalIndex64.Add(new AssemblyStoreHashEntry(reader, false));
            }
        }

        private void ReadHeader(BinaryReader reader)
        {
            if (reader.ReadUInt32() != AssemblyStoreMagic)
            {
                throw new InvalidOperationException("Invalid header magic number");
            }

            Version = reader.ReadUInt32();
            if (Version != AssemblyStoreFormatVersion)
            {
                throw new InvalidOperationException("Invalid Store Version");
            }

            LocalEntryCount = reader.ReadUInt32();
            GlobalEntryCount = reader.ReadUInt32();
            StoreId = reader.ReadUInt32();
        }

        private void LoadAssemblies(BinaryReader reader)
        {
            for (uint i = 0; i < LocalEntryCount; i++)
            {
                var assembly = new AssemblyStoreAssembly(reader, GetMemorySlice);
                Assemblies.Add(assembly);
            }
        }

        private ReadOnlyMemory<byte> GetMemorySlice(uint offset, uint length)
        {
            return _storeMemory.Slice((int)offset, (int)length);
        }

        public bool HasIdenticalContent(AssemblyStore other)
        {
            return
                other.Version == Version &&
                other.LocalEntryCount == LocalEntryCount &&
                other.GlobalEntryCount == GlobalEntryCount &&
                other.StoreId == StoreId &&
                other.Assemblies.Count == Assemblies.Count &&
                other.GlobalIndex32.Count == GlobalIndex32.Count &&
                other.GlobalIndex64.Count == GlobalIndex64.Count;
        }
    }
}