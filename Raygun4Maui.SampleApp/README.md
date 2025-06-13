# Raygun4Maui.SampleApp

Sample application for testing the Raygun4Maui SDK.

## Building for Release on macOS

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
dotnet publish -f:net8.0-ios -c Release /p:Platform=iPhone /p:ArchiveOnBuild=true
```

Validate the `.xcarchive`:

- Open Xcode >> Organizer
- Select "Raygun4Maui.SampleApp"
- Select a build and click the _"Validate"_ button

## Determing the UDID of a Binary File

1. Open Terminal
2. Run this command:

.dSYM

```bash
dwarfdump --uuid /path/to/YourFramework.dSYM
```

.framework

```bash
dwarfdump --uuid /path/to/YourFramework.framework
```
