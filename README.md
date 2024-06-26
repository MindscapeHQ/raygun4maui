# Raygun4Maui

Raygun's Crash Reporting and Real User Monitoring provider for .NET MAUI

## Installation

### Step 1 - Install Raygun4Maui

#### NuGet Package manager
The best way to install Raygun is to use the NuGet package manager. Right-click on your project and select "**Manage NuGet Packages....**". Navigate to the Browse tab, then use the search box to find **Raygun4Maui** and install it.

#### .NET Cli

To install the latest version:
``` 
dotnet add package Raygun4Maui
```

Alternatively, you can specify a version tag to install a specific version of the package. See [Raygun4Maui NuGet Gallery page](https://nuget.org/packages/Raygun4Maui) for information on available versions.
```
dotnet add package Raygun4Maui --version 2.0.1
```

### Step 2 - Add Raygun4Maui to your MauiApp builder

Import the module by:

``` csharp
using Raygun4Maui;
```

To activate sending of unhandled exceptions and logs to Raygun, you must add Raygun4Maui to your MauiApp builder. To do so, open your main MauiProgram class (MauiProgram.cs) and change the `CreateMauiApp` method by adding the `AddRaygun` extension method:

``` csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun();
```

The default method uses the configuration service to pull in your configuration and create the Raygun client

## Configuration

### Appsettings 

Configuration settings can be added via an appsettings.json file. To add appsettings.json to the bundled app you should add it as an embedded resource (consult IDE specific instructions). If you do not provide one we create a default Raygun4MauiSettings object which you can change using a lambda to change the options. This must be added to the configuration before you call the `.AddRaygun()` method.
```csharp
 var a = Assembly.GetExecutingAssembly();
 using var stream = a.GetManifestResourceStream("Raygun4Maui.SampleApp.appsettings.json");
        
 builder.Configuration.AddJsonStream(stream!);
```

Below is an example appsettings.json file, two key notes are that you need to use Raygun4MauiSettings as the configuration will not pull it in otherwise. Additionally, the RumFeatureFlags are comma seperated so that they can be loaded in correctly as a bitwise feature flag.

```json
{
  "Raygun4MauiSettings": {
    "RaygunSettings": {
      "ApiKey": "paste_your_api_key_here",
      "ApplicationVersion": "1.0.0",
    },
    "RaygunLoggerConfiguration": {
      "SendDefaultTags": true, 
      "SendDefaultCustomData": true,
      "MinLogLevel": "Debug",
      "MaxLogLevel": "Critical" 
    },
    "IgnoredViews": [
      "LoginView",
      "SettingsView"
    ],
    "IgnoredUrls": [
      "https://example.com/ignore"
    ],
    "EnableRealUserMonitoring": true,
    "RumFeatureFlags": "Network, Page, AppleNativeTimings"
  }
}
```

### Lambda Options

Mentioned previously, we provide an options lambda which you can use to make in-code changes to the configuration, e.g. 
```csharp
.AddRaygun(options => {
    options.RaygunSettings.ApiKey = "paste_your_api_key_here";
    options.EnableRealUserMonitoring = true;
    options.RumFeatureFlags = RumFeatures.Page | RumFeatures.Network | RumFeatures.AppleNativeTimings;
})
```

### Raygun4MauiSettings overload

The `AddRaygun` extension method contains an overloaded method that takes a `Raygun4MauiSettings` options object which can be used instead of the configuration service. This contains a `RaygunSettings` from [Raygun4Net](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/).

**Raygun4MauiSettings supports the following configurations:**
- RaygunSettings
  - Any configuration available in the Raygun4Net `RaygunSettings`, such as `ApiKey`.
- RaygunLoggerConfiguration
  - `SendDefaultTags` (defaulted to `true`) adds the Log Level (e.g., Severe, Warning, etc.) and the Build Platform (e.g., Windows, Android, iOS, etc.) to reports and logs sent to Raygun.
  - `SendDefaultCustomData` (defaulted to `true`) adds all available information in the uncaught exception as custom data on the crash report sent to Raygun.
  - `MinLogLevel` and `MaxLogLevel` that specify the range of logging levels to be sent to Raygun.
- `IgnoredViews` a list of views to ignore when tracking
- `IgnoredUrls` a list of URLs to ignore when tracking
- `RumApiEndpoint` endpoint to where the RUM data is sent
- `EnableRealUserMonitoring` to enable RUM 
- `RumFeatureFlags` a enum flag to enable specific RUM features, (e.g. RumFeatures.Page | RumFeatures.Network)


To use these additional configurations, create and initialize a new `RaygunLoggerConfiguration` object as follows:

``` csharp
Raygun4MauiSettings raygunMauiSettings = new Raygun4MauiSettings {
    RaygunSettings = new RaygunSettings() {
        ApiKey = "paste_your_api_key_here",
        
    },
    RaygunLoggerConfiguration = new RaygunLoggerConfiguration {
        SendDefaultTags = true, // defaults to true
        SendDefaultCustomData = true, // defaults to true
        MinLogLevel = LogLevel.Debug, // defaults to Debug
        MaxLogLevel = LogLevel.Critical // defaults to Critical
    }
    EnableRealUserMonitoring = true, // defaults to false
    RumFeatureFlags = RumFeatures.Page | RumFeatures.Network | RumFeatures.AppleNativeTimings // Enables Page, Network, and Native Apple Timings
};
```

The Raygun4MauiSettings are added to the service provider so that any DI dependent service can pull in the Raygun4MauiSettings and make changes. For example the application version may be obtained from an endpoint, so this can be assigned later rather than at instantiation.


### User Management
As part of Raygun4Net.NetCore v10.0.0, we are moving away from the use of UserInfo and User. 
These are now marked as obsolete, and within the Raygun4Maui provider we no longer support this.

We now have introduced the `IRaygunUserProvider`, which offers a `GetUser` function that our crash reporting can use to get the current user.
Only having GetUser makes sense for NetCore, but since MAUI supports RUM we need a way of notifying the RUM module that a user has changed.

We therefore, provide a `IRaygunMauiUserProvider` interface which adds a `SetUser` method. With this we can notify the RUM module. There is a default implementation of this class for this provider which takes in a user with `SetUser` and provides this user through `GetUser`.

You can obtain an instance of this provider through dependency injection using `IRaygunMauiUserProvider`, then you can set the user by calling `SetUser`.

```csharp
public MainPage(IRaygunMauiUserProvider userProvider) {
    userProivder.SetUser(new RaygunIdentifierMessage("anonymous");    
}
```

You can implement your own custom user provider if the default does not fit your needs. This can be done by implementing the `IRaygunMauiUserProvider`, specifically `GetUser` and `SetUser`.

Please note, if you create a custom implementation you must send a user changed event to the `RaygunAppEventPublisher` for our RUM module to be notified.

```csharp
RaygunAppEventPublisher.Publish(new RaygunUserChanged
{
    User = _user
});
```


As mentioned, we obtain this user provider by using dependency injection, so to add your instance of the user provider to the DI container we provide an extension on the app builder.

```csharp
builder.AddRaygunUserProvider<CustomRaygunMauiUserProvider>();
```

---

## Usage

Unhandled exceptions will be sent to Raygun automatically.

Raygun4Maui stores an instance of a `RaygunMauiClient` object that is initialized by the Maui builder, this can be accessed through the following code:
``` csharp
RaygunMauiClient.Current
```

This client extends the Raygun4Net.NetCore `RaygunClient`, as a result any features supported by the Raygun4Net.NetCore Client are supported here. 

---

### Manually sending exceptions
Raygun4Maui automatically sends unhandled exceptions. For manual sending, use `Send` or `SendInBackground` methods, as shown below:

``` csharp
try {   
    // Code that may fail 
} catch (Exception e) {   
    RaygunMauiClient.Current.SendInBackground(e);
//or
    RaygunMauiClient.Current.Send(e); 
}
```

An exception needs to be thrown in order for its stack trace to be populated. If the exception is created manually no stack trace data is collected. 

### Other examples
For additional examples on how to use the `RaygunMauiClient` object refer to the [Raygun4Net.NetCore  documentation](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/)
## ILogger logging
Raygun4Maui will automatically send any logger logs to Raygun.

To make a log entry, obtain the reference to the ILogger services that your MAUI app maintains:

``` csharp
ILogger logger = Handler.MauiContext.Services.GetService<ILogger<MainPage>>();
```

You may now use the appropriate ILogger log method from the logger object. This uses the same `RaygunMauiClient` object accessible from `RaygunMauiClient.Current`

```csharp
logger.LogInformation("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogInformation", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
logger.LogCritical("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogCritical", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
```

This functionality also allows you to manually catch and log exceptions as shown below:

``` csharp
try {
    // Code that throws exception
} catch (Exception e) {
    // Example ILogger call. You can use the ILogger method and arguments of your choice.
    logger.Log(LogLevel.Error, e, "Exception caught at {Timestamp}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
}
```

---
## Platform specific information
Raygun4Maui will automatically collect information specific to the environment the application is being run in. However, there are inconsistencies between certain values across platforms.
- on iOS, Raygun4Maui cannot obtain the device's name. This is a privacy restriction put in place by Apple. If you would like this information to be collected and sent with crash reports you will have to [request for permission from apple](https://developer.apple.com/documentation/bundleresources/entitlements/com_apple_developer_device-information_user-assigned-device-name?language=objc).
- The `Total physical memory` and `Available physical memory` properties mean different things across platforms. Below is a table explaining the differences for each platform.  

| Platform | Total physical memory                                                 | Available physical memory                                                                                                                                  |
|----------|-----------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Mac      | Total installed ram                                                   | Total memory available for user-level processes                                                                                                            |
| iOS      | Total installed ram                                                   | Total memory available for user-level processes                                                                                                            |
| Windows  | Total installed ram                                                   | Total amount of private memory used by the process at the time of the measurement. For a number of reasons this might not be the actual total memory usage |
| Android  | Total amount of memory that the JVM has allocated for the application | Total amount of free memory that the JVM has available for the application to use                                                                          | 


### Networking

| Platform | Networking support | Conditions                                    |
|----------|--------------------|-----------------------------------------------|
| Mac      | Yes                | HttpClient, NSURLSession, and NSURLConnection |
| iOS      | Yes                | HttpClient, NSURLSession, and NSURLConnection |
| Windows  | Yes                | HttpClient                                    |
| Android  | Yes                | HttpURLConnection (see SampleApp)             | 

---

## Offline Storage

You can optionally specify an Offline Store for crash reports when creating your `Raygun4MauiClient`.

When an offline store is specified, if there are any issues sending an exception to the Raygun API, a copy of the exception may be stored locally to be retried at a later date.

An exception is stored offline when one of the following conditions are met:
- There was a network connectivity issue, e.g. no active internet connection on a mobile device
- The Raygun API responded with an HTTP 5xx, indicating an unexpected server error


### Configuration

```csharp
// This will initialize Raygun with the default Application Data Store
mauiAppBuilder.AddRaygun(options => 
{
  options.UseOfflineStorage();
});
```

You can also define the background send strategy and store separately

```csharp
// Attempt to send any offline crash reports every 30 seconds
var sendStrategy = new TimerBasedSendStrategy(TimeSpan.FromSeconds(30));

// Store crash reports in directory relative to the Application (`FileSystem.AppDataDirectory`)
var offlineStore = new RaygunMauiOfflineStore(sendStrategy);

mauiAppBuilder.AddRaygun(options => 
{
  options.OfflineStore = offlineStore;
});
```

---
## Development Instructions

### To build a local nuget package
* Open Visual Studio 22+
* Open the raygun4maui.sln solution
* Right-click the project and select properties
* Ensure the produce a NuGet package build option is checked
* Under package, update the version name
Each time you build your project a .nupkg file will be created in your bin directory. This can then be used by following the following instructions

### Installing a local Nuget package
* Acquire the NuGet package or build it as discussed above
* Place it in a local folder of your choice
* Open your MAUI app inside of visual studio 22+
* Navigate to Options > Nuget Package Manager > Package Sources
* Create a new source by adding any name and the path to the folder where you placed the raygun4maui NuGet file
* Right-Click on the project and select Manage Nuget Packages
* Find the NuGet package by typing its name (Mindscape.Raygun4Maui) and, for simplicity, ensure only the newly created custom local package source is selected
You can also add local packages using the dotnet cli.

### Updating Local Nuget package
* Open your MAUI app inside of visual studio 22+
* Add the new Nuget file to your custom local package folder
* Right-Click on the project and select Manage Nuget Packages
* Find the currently installed version
* The system should automatically detect that a new version is available; click the update iconhttps://github.com/MindscapeHQ/raygun4net/tree/master/Mindscape.Raygun4Net.NetCore
