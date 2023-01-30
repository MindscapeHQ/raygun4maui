using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Mindscape.Raygun4Maui;
using Mindscape.Raygun4Net;
using Raygun4Maui.RaygunLogger;

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
        builder.Configuration.AddUserSecrets<MainPage>();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).AddRaygun4Maui();

        return builder.Build();
	}
}
