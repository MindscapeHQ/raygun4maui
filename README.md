# Raygun4Maui
==========

Coming soon! [Raygun](http://raygun.com) under-construction provider for .NET MAUI

! IMPORTANT: THIS PROVIDER IS UNDER DEVELOPMENT AND NOT READY FOR GENERAL USAGE !
====================

Build Local Nuget
====================
* Open Visual Studio 22+
* Open the raygun4maui.sln solution
* Right-click the project and select properties
* Ensure the produce a NuGet package on build option is checked
* Under package, update the version name
* Click to build the project
* If the building succeeds, the NuGet file will be created in the bin folder of the project

Installation via Local Nuget
====================
* Acquire the NuGet package or build it as discussed above
* Place it in a local folder
* Open Visual Studio 22+ and your MAUI app
* Create a new Nuget Package Source
* Navigate to Options > Nuget Package Manager > Package Sources
* Create a new source by adding any name and the path to the folder you placed the raygun4maui NuGet file
* Right-Click on the project and select Manage Nuget Packages
* Find the NuGet package by typing its name (Mindscape.Raygun4Maui) and, for simplicity, ensure only the newly created custom local package source is selected

Updating Local Nuget
====================
* Open Visual Studio 22+ and open your MAUI app
* Add the new Nuget file in the custom local package folder
* Right-Click on the project and select Manage Nuget Packages
* Find the currently installed version
* The system should automatically detect a new version is available; click the update icon

Usage
====================
* To activate (a) unhandled exceptions being sent to Raygun and (b) logging, you need to add raygun4maui to your MauiApp builder. To do so, open you main MauiProgram class (MauiProgram.cs) and change the CreateMauiApp method by adding the AddRaygun4Maui extension method:
```
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            ...
            .AddRaygun4Maui("YOUR_APP_API_KEY");
```
* To make a log entry:
* Acquire the reference to the ILogger services your MAUI app maintains:
```
ILogger logger = Handler.MauiContext.Services.GetService<ILogger<MainPage>>();
```
* Invoke the various ILogger log methods from the logger object accordingly. For example:
```
logger.LogInformation("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogInformation", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
logger.LogCritical("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogCritical", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
```
* You can also manually catch-and-log an exception as follows:
```
try
{
    //Code that throws exception
}
catch (Exception e)
{
    _logger.Log(LogLevel.Error, e, "Exception caught at {Timestamp}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
}
```
* If you require more fine-grain control on sending messages to Raygun, you can create a `RaygunClient` instance from the package `Mindscape.Raygun4Net`, which is already wrapped in this module, and then invoke the new client's Send, SendInBackground, etc., methods, as described in Raygun4Net.

Configuration
====================
* The system supports the following configurations:
* Any configuration available in Raygun4Net, such the ApiKey
* SendDefaultTags (default true) that adds the Log Level (e.g., Severe, Warning, etc.,) and the Build Platform (e.g., Windows, Android, iOS, etc.,) to messages sent to Raygun for (a) unhandled exceptions being sent to Raygun and (b) logging
* SendDefaultCustomData (default true) that adds all available information in the uncaught excpetions' callback as custome data and sends them to Raygun
* MinLogLevel and MaxLogLevel that specify the range of logging levels that will actually raise a message to Raygun
* These can be set as follows:
* The AddRaygun4Maui extension method and the RaygunMauiClient constructor have overloaded methods that can take as an input a RaygunMauiSettings options object, which extends RaygunSettings from Raygun4Net. Hence, both can be configured to run instead of the default values.
* Create and initialize a new RaygunMauiSettings object:
```
RaygunMauiSettings raygunMauiSettings = new RaygunMauiSettings(){
    ApiKey = "YOUR_APP_API_KEY",
    SendDefaultTags = true; //Default value
    SendDefaultCustomData = true; //Default value
    MinLogLevel = LogLevel.Debug; //Default value
    MaxLogLevel = LogLevel.Critical; //Default value
}
```
* To configure (a) unhandled exceptions being sent to Raygun and (b) logging:
```
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            ...
            .AddRaygun4Maui(raygunMauiSettings);
```
