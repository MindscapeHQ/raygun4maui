# Raygun4Maui

Raygun's Crash Reporting provider for .NET MAUI

## Version 1.1.0
This provider currently supports all Crash Reporting features that are supported by [Raygun4Net](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/) aswell as ILogger logging. 

---

## Installation

### Step 1 - Install Raygun4Maui

#### Nuget Package manager
The best way to install Raygun is to use the NuGet package manager. Right-click on your project and select "**Manage NuGet Packages....**". Navigate to the Browse tab, then use the search box to find **Raygun4Maui** and install it.

#### .Net Cli

To install the latest version:
``` 
dotnet add package Raygun4Maui
```

Alternitvely you can specify a version tag to install a specific version of the package, see [Raygun4Maui NuGet Gallery page](https://nuget.org/packages/Raygun4Maui) for information of versions. 
```
dotnet add package Raygun4Maui --version 1.0.0
```

### Step 2 - Add Raygun4Maui to your MauiApp builder

Import the module by:

``` c#
using Raygun4Maui;
```

To activate sending of unhandled exceptions and logs to Raygun, you must add Raygun4Maui to your MauiApp builder. To do so, open your main MauiProgram class (MauiProgram.cs) and change the `CreateMauiApp` method by adding the `AddRaygun4Maui` extension method:

``` c#
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun4Maui("paste_your_api_key_here");
```

## Additional configuration

The `AddRaygun4Maui` extension method and the `RaygunMauiClient` constructor have overloaded methods that can take a `RaygunMauiSettings` options object, which extends `RaygunSettings` from [Raygun4Net](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/net-core/).

**RaygunMauiSettings supports the following configurations:**
- Any configuration available in Raygun4Net's `RaygunSettings`, such as `ApiKey`.
- `SendDefaultTags` (default `true`) that adds the Log Level (e.g. Severe, Warning, etc.) and the Build Platform (e.g. Windows, Android, iOS, etc.) to reports and logs sent to Raygun.
- `SendDefaultCustomData` (default `true`) that adds all available information in the uncaught exception's callback as custom data on the crash report sent to Raygun.
- `MinLogLevel` and `MaxLogLevel` that specify the range of logging levels to be sent to Raygun.

To use these additional configurations, create and initialize a new `RaygunMauiSettings` object:

``` c#
Raygun4MauiSettings raygunMauiSettings = new Raygun4MauiSettings {
    ApiKey = "paste_your_api_key_here",
    SendDefaultTags = true, // Default value
    SendDefaultCustomData = true, // Default value
    MinLogLevel = LogLevel.Debug, // Default value
    MaxLogLevel = LogLevel.Critical // Default value
};
```

Then add Raygun4Maui to your MauiApp builder, this time passing in the `RaygunMauiSettings` object instead of the API key directly:

``` c#
builder
    .UseMauiApp<App>()
    ...
    .AddRaygun4Maui(raygunMauiSettings);
```

---

## Usage

Unhandled exceptions will be sent to Raygun automatically.

Raygun4Maui stores a instance of a Raygun4net RaygunClient object, this can be accessed through the following code:
``` c#
RaygunMauiClient.Current
```

Any features supported by the Raygun4net Client are accessible here

---

### Manually sending exceptions
Raygun4Maui automatically sends unhandled exceptions. For handled exceptions, use Send or SendInBackground methods, as shown below:

``` c#
try {   
// Code that may fail 
} catch (Exception e) {   
RaygunMauiClient.Current.SendInBackground(e);
//or
RaygunMauiClient.Current.Send(e); 
}
```

#### Throw exceptions to avoid missing stack traces
An exception needs to be thrown in order for its stack trace to be populated like the example above. If you create and send a new exception instance to Raygun then it will not contain any stack trace information

---

### Modify or cancel messages
Attach an event handler to the SendingMessage event on the RaygunClient. This allows you to modify or cancel the RaygunMessage before it's sent.
Here's an example of canceling a message:
``` c#
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

        var configuration = new ConfigurationBuilder()
       .AddUserSecrets<MainPage>()
       .Build();
       
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.AddRaygun4Maui('API_KEY');

        RaygunMauiClient.Current.SendingMessage += OnSendingMessage;

		return builder.Build();
	}

    private static void OnSendingMessage(object sender, RaygunCustomGroupingKeyEventArgs e)
    {
		// This is an example of cancelling a message: 
		if ("System.NotImplementedException".Equals(e.Message.Details.Error.ClassName)) { 
			e.Cancel = true; 
		}
    }
}
```

---

### Custom grouping
When an exception report is send to Raygun, they are grouped using our classification logic. If for any reason you want to customize specific grouping behavior you can use the CustomGroupingKey event.

Attach an event handler to the CustomGroupingKey event on the RaygunClient. This lets you define your own grouping logic for exceptions.

Here is an example of providing a custom grouping key:
``` c#
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

        var configuration = new ConfigurationBuilder()
       .AddUserSecrets<MainPage>()
       .Build();
       
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.AddRaygun4Maui('API_KEy');

        RaygunMauiClient.Current.CustomGroupingKey += CustomGroupingKey;

		return builder.Build();
	}

    private static void CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
    {
	    // This example simply performs pure message based grouping on basic Exception instances:
		 if (e.Message.Details.Error.ClassName.Equals("System.Exception")) { 
			 string key = e.Exception.Message; 
			 e.CustomGroupingKey = key; 
		 }    
    }
}
```
Note that if an exception occurs within your CustomGroupingKey event handler, Raygun4Maui will detect this and send the new exception as well as the original exception that was being processed. When processing the new exception, the event handler will not be called again in order to avoid an infinite loop.

---

### Unique user tracking
By providing user information, your Raygun dashboard can display the number of unique users affected by each error or crash. This helps prioritize issues with the greatest impact. Make sure to follow your company's privacy policies when providing user information.

At minimum, provide a unique guid to see the number of affected users. You could also provide a meaningful ID, such as a database ID, or even a name and contact details to inform customers about resolved or ongoing issues.

There are several ways to provide user information. One simple method is to set the User property of the RaygunClient instance with an identifier:

``` c#
RaygunMauiClient.Current.User = "[user@email.com](mailto:user@email.com)";
```

If a single identifier isn't enough, set the UserInfo property to provide more details:

``` c#
RaygunMauiClient.Current.UserInfo = new RaygunIdentifierMessage("[user@email.com](mailto:user@email.com)") { 
IsAnonymous = false, 
FullName = "Robbie Robot", 
FirstName = "Robbie" 
};
```

RaygunIdentifierMessage properties include:

-   Identifier: unique identifier for this user from your system.
-   IsAnonymous: flag indicating if the user is logged in or anonymous (unique identifier still possible).
-   Email: user's email address. If using email as the identifier, leave this blank.
-   FullName: user's full name.
-   FirstName: user's first or preferred name.
-   UUID: device identifier for tracking users across devices or identifying problematic machines.

Another way to provide user information is through Send or SendInBackground overloads with a RaygunIdentifierMessage parameter. Use this when user information is only available during a crash, isn't fixed, or in heavily threaded scenarios with separate RaygunClient instances:
``` c#
RaygunMauiClient.Current.SendInBackground(exception, null, null, new RaygunIdentifierMessage("[user@email.com](mailto:user@email.com)") { 
	IsAnonymous = false, 
	FullName = "Robbie Robot", 
	FirstName = "Robbie" 
});
```


User string properties have a 255-character limit. Users with longer fields won't be processed.

---

### Tags
Overloads of the Send and SendInBackground methods allow you to include a list of tags with each manually sent exception:

```csharp
RaygunMauiClient.Current.Send(exception, new List<string>() { "tag1", "tag2" });
```

Exceptions can be searched in your Raygun dashboard by tags that you've included.

---

### Custom data
You can include key-value custom data using an overload of the Send and SendInBackground methods. Values can be primitive types or rich object structures. All properties of objects and their children will be sent to Raygun. Cyclic object references will be detected and handled as appropriate, and any property getter that produces an exception will cause that property value to be displayed as the exception message:

```csharp
RaygunMauiClient.Current.Send(exception, null, new Dictionary<string, object>() { { "key", "value" } });
```

The second parameter is the list of tags (mentioned above) which can be null if you don't have tags for the current exception.

---

### Version numbering
By default, raygun4maui will attempt to automatically send the version of your project. If this does not work you can set your own custom version value:
``` c#
RaygunMauiClient.Current.ApplicationVersion = "0.0.0.0";
```

---

### Stripping wrapper exceptions
Use AddWrapperExceptions to specify any outer exceptions that you want to strip:
``` c#
RaygunMauiClient.Current.AddWrapperExceptions(typeof(MyWrapperException));
```

Alternatively you can prevent Raygun4Maui from stripping exceptions by using RemoveWrapperExceptions:
```c#
RaygunMauiClient.Current.RemoveWrapperExceptions(typeof(MyWrapperException));
```

---

## ILogger logging

To make a log entry, acquire the reference to the ILogger services that your MAUI app maintains:

``` c#
ILogger logger = Handler.MauiContext.Services.GetService<ILogger<MainPage>>();
```

You may now invoke the various ILogger log methods from the logger object accordingly, this will use the RaygunMauiClient.Current client object for sending reports to Raygun. This means that any properties you set on the client, such as user info, will be sent by the logger. 
Here is an example of using the logger:

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
* The system should automatically detect that a new version is available; click the update icon
