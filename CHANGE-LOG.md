# Full Change Log for Raygun4Maui package

### v2.0.0
Adds support for Real User Monitoring (RUM) for Windows, Android, iOS, and MacCatalyst

#### Implemented RUM Features:
- Page Tracking
- Page Load Times
  - Time between PageDisappearing and PageAppearing 
- Session Tracking
- Custom Timings
  - Supports "t" type timings
- Native iOS Timings
  - iOS/MacCatalyst specific
- Network Timings
  - See README.md for support information

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
  - `IgnoredUrls` a list of url's to ignore when tracking
  - `RumApiEndpoint` endpoint to where the RUM data is sent
  - `EnableRealUserMonitoring` to enable RUM - defaults to `true`
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
