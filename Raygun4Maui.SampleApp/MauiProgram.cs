using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Maui;
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
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.AddRaygun4Maui(options =>
			{
				options.ApiKey = apiKey;
			});

		return builder.Build();
	}
}
