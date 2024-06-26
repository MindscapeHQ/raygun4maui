﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
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
            .AddRaygun(options =>
            {
                options.UseOfflineStorage();
            });


        builder.Services.AddSingleton<MainPage>();
        
        return builder.Build();
    }
}