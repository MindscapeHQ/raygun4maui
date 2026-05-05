# Raygun4Maui.SampleApp

Sample application for testing the Raygun4Maui SDK.

## Test against a pre-release NuGet (instead of a ProjectReference)

By default the sample app references `Raygun4Maui` via `<ProjectReference>` for fast
iteration. Before shipping a release, validate the SDK as it will actually be consumed
by customers — i.e. as a NuGet package. A `ProjectReference` skips the packaging step,
so it does not exercise:

- `GeneratePackageOnBuild` output (what ends up in the `.nupkg`)
- The `Raygun4Maui.Platform` package's `buildTransitive` targets
- Native asset packing (`networkmonitorlibrary.aar`, `RaygunNetworkMonitor.framework`)
- TFM-conditional `PackageReference`s the main project declares for each platform

To catch packaging regressions, switch the sample app to a `PackageReference` against a
locally-built pre-release `.nupkg`:

### 1. Bump the shared version to a pre-release

In `Directory.Build.props`, add a pre-release suffix, e.g. `3.0.1-pre.1`:

```xml
<Version>3.0.1-pre.1</Version>
```

### 2. Build the pre-release packages

The repo's NuGet config already registers `./.nuget-local` as the user-level
`Raygun4MauiLocal` source (verify with `dotnet nuget list source`). Build the two
shipping packages and copy them into that folder:

```bash
dotnet build Raygun4Maui.Platform/Raygun4Maui.Platform.csproj -c Release
cp Raygun4Maui.Platform/bin/Release/Raygun4Maui.Platform.<version>.nupkg .nuget-local/

dotnet build Raygun4Maui/Raygun4Maui.csproj -c Release
cp Raygun4Maui/bin/Release/Raygun4Maui.<version>.nupkg .nuget-local/

# Force NuGet to re-read the local feed
dotnet nuget locals http-cache --clear
```

### 3. Swap the sample app's reference

In `Raygun4Maui.SampleApp.csproj`, replace the `<ProjectReference>` to
`Raygun4Maui.csproj` with a `<PackageReference>` at the pre-release version:

```xml
<PackageReference Include="Raygun4Maui" Version="3.0.1-pre.1" />
```

After testing, revert the version bump in `Directory.Build.props` and restore the
`<ProjectReference>` so day-to-day development still uses the in-tree project.

## Build and publish sample app for Xcode validation

List the available code-signing keys:

```bash
security find-identity -p codesigning -v
```

This will output something like:

```bash
1) ABC1234567890 "Apple Development: John Doe (XXXXXXXXXX)"
2) DEF0987654321 "Apple Distribution: John Doe (YYYYYYYYYY)"
```

Use the full string in quotes as the `CodesignKey` in the .csproj:

```csharp
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|iPhone'">
    <CodesignKey>Apple Distribution: John Doe (YYYYYYYYYY)</CodesignKey>
</PropertyGroup>
```

Publish the `.xcarchive`:

```bash
dotnet publish -f:net10.0-ios -c Release -r ios-arm64 /p:ArchiveOnBuild=true /p:Platform=iPhone
```

Validate the `.xcarchive`:

- Open Xcode >> Organizer
- Select "Raygun4Maui.SampleApp"
- Select a build and click the _"Validate"_ button

## Using dwarfdump to retrieve UUIDs

1. Open Terminal
2. Run this command:

.dSYM

```bash
dwarfdump --uuid /path/to/RaygunNetworkMonitor.dSYM
```

.framework

```bash
dwarfdump --uuid /path/to/RaygunNetworkMonitor.framework
```
