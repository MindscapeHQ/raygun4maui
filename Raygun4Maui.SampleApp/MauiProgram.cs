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
        
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("Raygun4Maui.SampleApp.appsettings.json");
        
        builder.Configuration.AddJsonStream(stream!);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Raygun(builder.Configuration["Raygun4MauiSettings:RaygunSettings:apiKey"])
            .CreateLogger();
        

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .AddRaygun();

        return builder.Build();
    }
}