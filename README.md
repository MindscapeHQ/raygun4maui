# Raygun4Maui

Raygun's Crash Reporting provider for .NET MAUI

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
dotnet add package Raygun4Maui --version 1.2.1
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

## Additional configuration

We provide an options lambda which you can use to make in-code changes to the configuration, e.g. 
```csharp
.AddRaygun(options => {...})
```

The `AddRaygun` extension method contains an overloaded method that takes a `Raygun4MauiSettings` options object which can be used instead of the configuration service. This contains a `RaygunSettings` from [Raygun4Net](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/).

**Raygun4MauiSettings supports the following configurations:**
- Any configuration available in the Raygun4Net `RaygunSettings`, such as `ApiKey`.
- `SendDefaultTags` (defaulted to `true`) adds the Log Level (e.g., Severe, Warning, etc.) and the Build Platform (e.g., Windows, Android, iOS, etc.) to reports and logs sent to Raygun.
- `SendDefaultCustomData` (defaulted to `true`) adds all available information in the uncaught exception as custom data on the crash report sent to Raygun.
- `MinLogLevel` and `MaxLogLevel` that specify the range of logging levels to be sent to Raygun.
- `EnableRealUserMonitoring` to enable RUM 
- `RumFeatureFlags` a enum flag to enable specific RUM features, (e.g. RumFeatures.Page | RumFeatures.Network)


To use these additional configurations, create and initialize a new `RaygunLoggerConfiguration` object as follows:

``` csharp
Raygun4MauiSettings raygunMauiSettings = new Raygun4MauiSettings {
    RaygunSettings = new RaygunLoggerConfiguration() {
        ApiKey = "paste_your_api_key_here",
        SendDefaultTags = true, // defaults to true
        SendDefaultCustomData = true, // defaults to true
        MinLogLevel = LogLevel.Debug, // defaults to true
        MaxLogLevel = LogLevel.Critical // defaults to true
    },
    EnableRealUserMonitoring = true, // defaults to true
    RumFeatureFlags = RumFeatures.Page | RumFeatures.Network // Enables Page and Network tracking
};
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

| Platform | Total physical memory | Available physical memory |
| ----- | ---- | ------- |
| Mac  | Total installed ram | Total memory available for user-level processes  |
| iOS | Total installed ram | Total memory available for user-level processes |
| Windows |Total installed ram | Total amount of private memory used by the process at the time of the measurement. For a number of reasons this might not be the actual total memory usage |
| Android |Total amount of memory that the JVM has allocated for the application | Total amount of free memory that the JVM has available for the application to use | 


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
