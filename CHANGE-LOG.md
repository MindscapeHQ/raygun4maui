# Full Change Log for Raygun4Maui package

### v2.1.1
- Version bump to `Raygun4Net.NetCore v11.0.1`
  - This marks the synchronous `Send()` methods as `Obsolete` which can cause deadlocks in MAUI if sending on the UI Thread
- Added support for reading PDB debug information from Android Assembly Store
  - This completes the portable pdb support for MAUI, and Android targets are fully compatible
- Introduce dependency on `K4os.Compression.LZ4@1.3.8` for Android
  - This is to support decompression of LZ4 binaries in the Assembly Blob Store

### v2.1.0
- Version bump to `Raygun4Net.NetCore v11.0.0`
- Added support for capturing debug information for PDB symbolication
  - This is automatically captured for supported platforms
  - Currently, there is limited Android support, due to the changes with single file assembly stores
- Added support for storing crash reports offline when exceptions fail to send to the Raygun API
  - See [README.md](https://github.com/MindscapeHQ/raygun4maui/blob/master/README.md#offline-storage) for more information and usage documentation

### v2.0.1
Version bump to Raygun4Net.NetCore v10.1.2
- Fix issue where uncaught exceptions could sometimes not be reported to Raygun
  - See: https://github.com/MindscapeHQ/raygun4net/pull/529
  - Keeps a strong reference to the `OnApplicationUnhandledException` delegate in the client so that reference is alive as long as the client is

### v2.0.0
Adds support for Real User Monitoring (RUM) for Windows, Android, iOS, and MacCatalyst

#### Implemented RUM Features:
- Page Tracking
- Page Load Times
  - Time between PageDisappearing and PageAppearing 
- Session Tracking
- Custom Timings
- Native iOS Timings
  - iOS/MacCatalyst specific
- Network Timings
  - See [README.md](https://github.com/MindscapeHQ/raygun4maui/blob/master/README.md) for support information

#### Provider Changes
- `Raygun4MauiSettings` no longer inherits from `RaygunLoggerConfiguration`
  - `RaygunSettings` and `RaygunLoggerConfiguration` are now child elements
  - `RaygunLoggerConfiguration` no longer inherits `RaygunSettings`
- Changes `.AddRaygun4Maui` to `.AddRaygun`
  - Introduces `appsettings.json` style configuration options
  - Adds overload for `appsettings.json` configuration method with an `Action<Raygun4MauiSettings>` to change settings at runtime
  - Keeps overload for `.AddRaygun(Raygun4MauiSettings)` - other two are more recommended
- Adds more configuration options for RUM features
  - `IgnoredViews` a list of views to ignore when tracking
  - `IgnoredUrls` a list of URLs to ignore when tracking
  - `RumApiEndpoint` endpoint to where the RUM data is sent
  - `EnableRealUserMonitoring` to enable RUM - defaults to `false`
  - `RumFeatureFlags` a enum flag to enable specific RUM features, (e.g. RumFeatures.Page | RumFeatures.Network)
- Adds `Raygun4MauiSettings` to service provider for DI dependent services to edit it
- Includes fixes from Raygun4Maui 1.4.1 and 1.4.2 
  - 1.4.1
    - Fixes Android native crash due to concurrent access to environment information
  - 1.4.2 
    - Fixes issues on iOS when sending a crash report on another thread
    - Fixes Android crash when ILogger is spammed with logs/errors
- Minor version bump for Raygun4Net.NetCore to 8.2.0
  - Raygun4Net.NetCore now handles the unhandled exceptions
  - Introduces `ThrottledBackgroundMessageProcessor`
- Major version bump for Raygun4Net.NetCore to v10.0.0 see [changes](https://github.com/MindscapeHQ/raygun4net/blob/master/CHANGE-LOG.md)
  - Brings in IRaygunUserProvider which we wrap in an abstract class called RaygunMauiUserProvider
  - Obsoletes `User` and `UserInfo` in the Raygun client
    - `User` and `UserInfo` are no longer supported in Raygun4Maui
  - Fixes uninstantiated fields in Raygun message causing null pointers
  - Fixes situations where Raygun client settings did not reflect the `RaygunSettings` object
- Minor version bump for Raygun4Net.NetCore v10.1.0
  - Allows environment variables to be included in the crash report see [changes](https://github.com/MindscapeHQ/raygun4net/pull/523)


### v1.4.2
- Fixed issue with SendInBackground where environment variables are collected on the wrong thread causing it to fail silently

### v1.4.1
- Fixed issue with resource locking of device environment information on Android causing app to crash

### v1.4.0
- Dependency update to Raygun4Net 8.0.0

### v1.3.0
- Dependency update to Raygun4Net 7.1.0
