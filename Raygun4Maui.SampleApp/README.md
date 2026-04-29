# Raygun4Maui.SampleApp

Sample application for testing the Raygun4Maui SDK.

## Build pre-release packages

### Add pre-release suffix e.g. `3.0.0-preview1`:

- `Directory.Build.props`

### Add a local package source NuGet.config

- Create `/.nuget-local` folder in root directory
- Add `nuget.config` to root directory:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="Local Packages" value="./Packages" />
  </packageSources>
</configuration>
```

### Build and publish the pre-release packages

```bash
Raygun4Maui.Platform % dotnet build -c Release
# Copy bin/Release/Raygun4Maui.Platform.x.x.x.nupkg to /.nuget-local

Raygun4Maui % dotnet build -c Release
# Copy bin/Release/Raygun4Maui.x.x.x.nupkg to /.nuget-local
```

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
