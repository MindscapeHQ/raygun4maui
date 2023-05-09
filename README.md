# Raygun4Maui

Raygun's Crash Reporting provider for .NET MAUI

## Installation

### Step 1 - Install Raygun4Maui

#### Nuget Package manager
The best way to install Raygun is to use the NuGet package manager. Right-click on your project and select "**Manage NuGet Packages....**". Navigate to the Browse tab, then use the search box to find **Raygun4Maui** and install it.

#### .NET Cli

To install the latest version:
``` 
dotnet add package Raygun4Maui
```

Alternatively you can specify a version tag to install a specific version of the package, see [Raygun4Maui NuGet Gallery page](https://nuget.org/packages/Raygun4Maui) for information of versions. 
```
dotnet add package Raygun4Maui --version 1.2.0
```

### Step 2 - Add Raygun4Maui to your MauiApp builder

Import the module by:

``` csharp
using Raygun4Maui;
```

To activate sending of unhandled exceptions and logs to Raygun, you must add Raygun4Maui to your MauiApp builder. To do so, open your main MauiProgram class (MauiProgram.cs) and change the `CreateMauiApp` method by adding the `AddRaygun4Maui` extension method:

``` csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun4Maui("paste_your_api_key_here");
```

## Additional configuration

The `AddRaygun4Maui` extension method contains an overloaded method that takes a `RaygunMauiSettings` options object. This extends `RaygunSettings` from [Raygun4Net](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/).

**RaygunMauiSettings supports the following configurations:**
- Any configuration available in the Raygun4Net `RaygunSettings`, such as `ApiKey`.
- `SendDefaultTags` (defaulted to `true`) adds the Log Level (e.g., Severe, Warning, etc.) and the Build Platform (e.g., Windows, Android, iOS, etc.) to reports and logs sent to Raygun.
- `SendDefaultCustomData` (defaulted to `true`) adds all available information in the uncaught exception as custom data on the crash report sent to Raygun.
- `MinLogLevel` and `MaxLogLevel` that specify the range of logging levels to be sent to Raygun.

To use these additional configurations, create and initialize a new `RaygunMauiSettings` object as follows:

``` csharp
Raygun4MauiSettings raygunMauiSettings = new Raygun4MauiSettings {
    ApiKey = "paste_your_api_key_here",
    SendDefaultTags = true, // Default value
    SendDefaultCustomData = true, // Default value
    MinLogLevel = LogLevel.Debug, // Default value
    MaxLogLevel = LogLevel.Critical // Default value
};
```

Then add Raygun4Maui to your MauiApp builder. This time, passing in the `RaygunMauiSettings` object instead of the API key directly:

``` csharp
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun4Maui(raygunMauiSettings);
```

---

## Usage

Unhandled exceptions will be sent to Raygun automatically.

Raygun4Maui stores an instance of a `RaygunClient` object, this can be accessed through the following code:
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
For aditional examples on how to use the `RaygunClient` object refer to the [Raygun4Net.NetCore  documentation](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/)
## ILogger logging
Raygun4Maui will automatically send any logger logs to Raygun.

To make a log entry, obtain the reference to the ILogger services that your MAUI app maintains:

``` c#
ILogger logger = Handler.MauiContext.Services.GetService<ILogger<MainPage>>();
```

You may now invoke the various ILogger log methods from the logger object accordingly. This uses the same `RaygunClient` object accessible from `RaygunMauiClient.Current`

```c#
logger.LogInformation("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogInformation", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
logger.LogCritical("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogCritical", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
```

With this functionality, you can also manually catch-and-log exceptions as follows:

``` c#
try {
    // Code that throws exception
} catch (Exception e) {
    // Example ILogger call. You can use the ILogger method and arguments of your choice.
    logger.Log(LogLevel.Error, e, "Exception caught at {Timestamp}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
}
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
