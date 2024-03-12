# Full Change Log for Raygun4Maui package

### v2.0.0
Added support for real user monitoring (RUM) for Windows, Android, iOS, and MacCatalyst
Implemented features:
- Page Tracking
- Page Load Times
- Session Tracking
- Custom Timings
- Native iOS Timings
- Network Timings

RC-4:
- Windows, iOS, and Android network timings (see README)
- Fixed Android environment locking issue causing app to crash (1.4.1)
- Bumped Raygun4Net version to 8.2.0, moves responsibility of unhandled exceptions
- 
RC-5
- MacCatalyst network timings

RC-6
- Fixes native binding inclusion in NuGet package
- Increases minimum .NET version to .NET 7 as .NET 6 is now unsupported

RC-7
- Major version bump for Raygun4Net.NetCore to v10.0.0 see [changes](https://github.com/MindscapeHQ/raygun4net/blob/master/CHANGE-LOG.md)
  - Brings in IRaygunUserProvider which we wrap in an abstract class called RaygunMauiUserProvider
  - Obsoletes UserInfo and User in the Raygun client
    - User and UserInfo are no longer supported in Raygun4Maui
  - Fixes uninstantiated fields in Raygun message causing null pointers
  - Fixes situations where Raygun client settings did not reflect the RaygunSettings object
- Brings in fixes from v1.4.2
  - Fixes SendInBackground so it can work on iOS again
  - Fixes situations where unhandled exceptions are not reported on iOS (related to above)
  - Fixes an issue with spamming of the ILogger causing Android to crash
- Adds Raygun4MauiSettings to service provider for DI dependent services to edit it

### v1.4.2
- Fixed issue with SendInBackground where environment variables are collected on the wrong thread causing it to fail silently

### v1.4.1
- Fixed issue with resource locking of device environment information on Android causing app to crash

### v1.4.0
- Dependency update to Raygun4Net 8.0.0

### v1.3.0
- Dependency update to Raygun4Net 7.1.0
