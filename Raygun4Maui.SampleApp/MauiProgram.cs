using Microsoft.Extensions.Configuration;
using Raygun4Maui;

namespace Raygun4Maui.SampleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var configuration = new ConfigurationBuilder()
       .AddUserSecrets<MainPage>()
       .Build();

        string apiKey = configuration["apiKey"] ?? "";

        var builder = MauiApp.CreateBuilder();
        //builder.Configuration.AddUserSecrets<MainPage>();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).AddRaygun4Maui(apiKey);
        return builder.Build();    
    }
}
