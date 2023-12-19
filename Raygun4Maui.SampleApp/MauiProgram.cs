using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls.Hosting;
using Mindscape.Raygun4Net;
using Raygun4Maui.Raygun4Net.RaygunLogger;
using Serilog;

namespace Raygun4Maui.SampleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Raygun(apiKey)
            .CreateLogger();


        builder.Configuration.AddJsonFile("appsettings.json");

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .AddRaygun(RaygunOptions);

        return builder.Build();
    }
}