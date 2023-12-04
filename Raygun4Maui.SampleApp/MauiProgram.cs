using Microsoft.Extensions.Configuration;
using Mindscape.Raygun4Net;
using Serilog;

namespace Raygun4Maui.SampleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<MainPage>()
            .Build();

        var apiKey = configuration["apiKey"] ?? "";

        var builder = MauiApp.CreateBuilder();
        //builder.Configuration.AddUserSecrets<MainPage>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Raygun(apiKey)
            .CreateLogger();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).AddRaygun(new Raygun4MauiSettings(apiKey));
        
        return builder.Build();
    }
}