using System.Reflection;
using Microsoft.Extensions.Configuration;
using Mindscape.Raygun4Net.Extensions.Logging;
using Raygun4Maui.SampleApp.TestingLogic;
using Serilog;

namespace Raygun4Maui.SampleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("Raygun4Maui.SampleApp.appsettings.json");
        
        builder.Configuration.AddJsonStream(stream!);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Raygun(builder.Configuration["Raygun4MauiSettings:ApiKey"])
            .CreateLogger();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .AddRaygun(options =>
            {
                options.UseOfflineStorage();
            })
            .AddRaygunUserProvider<RaygunMauiUserProvider>();

        // Send errors logged without an exception too, so TestLoggerErrorsSent reaches Raygun
        builder.Logging.AddRaygunLogger(options =>
        {
            options.OnlyLogExceptions = false;
        });

        builder.Services.AddSingleton<MainPage>();
        
        return builder.Build();
    }
}