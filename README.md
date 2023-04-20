# Raygun4Maui

Raygun's Crash Reporting provider for .NET MAUI

## Version 1.0.0
This provider has been released with limited functionality. It currently supports reporting of unhandled exceptions and ILogger logging. At this time, testing has only been conducted on Windows. If you have any questions or concerns, please [contact support](https://raygun.com/about/contact) for additional assitance.

---

## Installation

### Step 1 - Install Raygun4Maui

The best way to install Raygun is to use the NuGet package manager. Right-click on your project and select "**Manage NuGet Packages....**". Navigate to the Browse tab, then use the search box to find **Raygun4Maui** and install it.

Alternatively, visit the [Raygun4Maui NuGet Gallery page](https://nuget.org/packages/Raygun4Maui) for instructions on installation using the package manager console.

### Step 2 - Add Raygun4Maui to your MauiApp builder

Import the module by:

```
using Raygun4Maui;
```

To activate sending of unhandled exceptions and logs to Raygun, you must add Raygun4Maui to your MauiApp builder. To do so, open you main MauiProgram class (MauiProgram.cs) and change the `CreateMauiApp` method by adding the `AddRaygun4Maui` extension method:

```
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun4Maui("paste_your_api_key_here");
```

## Additional configuration

The `AddRaygun4Maui` extension method and the `RaygunMauiClient` constructor both contain overloaded methods that can take a `RaygunMauiSettings` options object. This extends `RaygunSettings` from [Raygun4Net](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/).

**RaygunMauiSettings supports the following configurations:**
- Any configuration available in the Raygun4Net provider's `RaygunSettings`, such as `ApiKey`.
- `SendDefaultTags` (defaulted to `true`) which adds the Log Level (e.g. Severe, Warning, etc.) and the Build Platform (e.g. Windows, Android, iOS, etc.) to reports and logs sent to Raygun.
- `SendDefaultCustomData` (defaulted to `true`) which adds all available information in the uncaught exception's callback as custom data on the crash report sent to Raygun.
- `MinLogLevel` and `MaxLogLevel` which specifies the range of logging levels to be sent to Raygun.

To use these additional configurations, create and initialize a new `RaygunMauiSettings` object as follows:

```
Raygun4MauiSettings raygunMauiSettings = new Raygun4MauiSettings {
    ApiKey = "paste_your_api_key_here",
    SendDefaultTags = true, // Default value
    SendDefaultCustomData = true, // Default value
    MinLogLevel = LogLevel.Debug, // Default value
    MaxLogLevel = LogLevel.Critical // Default value
};
```

Then add Raygun4Maui to your MauiApp builder. This time, passing in the `RaygunMauiSettings` object instead of the API key directly:

```
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun4Maui(raygunMauiSettings);
```

---

## Usage

Unhandled exceptions will be sent to Raygun automatically.

## ILogger logging

To make a log entry, acquire the reference to the ILogger services that your MAUI app maintains:

```
ILogger logger = Handler.MauiContext.Services.GetService<ILogger<MainPage>>();
```

You may now invoke the various ILogger log methods from the logger object accordingly. For example:

```
logger.LogInformation("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogInformation", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
logger.LogCritical("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogCritical", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
```

With this functionality, you can also manually catch-and-log exceptions as follows:

```
try {
    // Code that throws exception
} catch (Exception e) {
    // Example ILogger call. You can use the ILogger method and arguments of your choice.
    _logger.Log(LogLevel.Error, e, "Exception caught at {Timestamp}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
}
```

---

## Extended functionality

If you require more fine-grain control over sending messages to Raygun, you can create a `RaygunClient` instance from the package `Mindscape.Raygun4Net`, which is already wrapped in this module. 

This gives you access to underlying methods such as `Send` and `SendInBackground`, as described in our [Raygun for .NET Core docs](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/).

---

## Development

### Build Local Nuget
* Open Visual Studio 22+
* Open the raygun4maui.sln solution
* Right-click the project and select properties
* Ensure the produce a NuGet package build option is checked
* Under package, update the version name
* Click to build the project
* If the building succeeds, the NuGet file will be created in the bin folder of the project

### Installation via Local Nuget
* Acquire the NuGet package or build it as discussed above
* Place it in a local folder
* Open Visual Studio 22+ and your MAUI app
* Create a new Nuget Package Source
* Navigate to Options > Nuget Package Manager > Package Sources
* Create a new source by adding any name and the path to the folder where you placed the raygun4maui NuGet file
* Right-Click on the project and select Manage Nuget Packages
* Find the NuGet package by typing its name (Mindscape.Raygun4Maui) and, for simplicity, ensure only the newly created custom local package source is selected

### Updating Local Nuget
* Open Visual Studio 22+ and your MAUI app
* Add the new Nuget file in the custom local package folder
* Right-Click on the project and select Manage Nuget Packages
* Find the currently installed version
* The system should automatically detect that a new version is available; click the update icon
