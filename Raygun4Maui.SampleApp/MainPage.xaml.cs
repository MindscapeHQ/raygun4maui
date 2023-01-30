namespace Raygun4Maui.SampleApp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Raygun4Maui.RaygunLogger;
using Raygun4Maui.SampleApp.TestingLogic;

public partial class MainPage : ContentPage
{
	int count = 0;

    private readonly String _apiKey;

    public MainPage()
	{
		InitializeComponent();

        var configuration = new ConfigurationBuilder()
               .AddUserSecrets<MainPage>()
               .Build();

        _apiKey = configuration["apiKey"] ?? "";

        if (_apiKey != "")
        {
            ApiKeyLabel.Text += _apiKey;
        }
        else
        {
            ApiKeyLabel.Text += "Not set! Please set it Right-Click on the project solution and selecting Manage User Secrets";
        }
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	private void OnManualExceptionClicked(object sender, EventArgs e)
	{
        ManualExceptionButton.Text += ".";

        TestManualExceptionsSent testManualExceptionsSent = new(_apiKey);

        testManualExceptionsSent.RunAllTests();
    }

    private void OnUnhandledExceptionClicked(object sender, EventArgs e)
    {
        UnhandledExceptionButton.Text += ".";

        TestUnhandledExceptionsSent testUnhandledExceptionsSent = new(_apiKey);
        testUnhandledExceptionsSent.RunAllTests();
    }

    private void OnILoggerErrorClicked(object sender, EventArgs e)
    {
        ILoggerButton.Text += ".";

        //  using IHost host = Host.CreateDefaultBuilder().Build();

        // var logger = host.Services.GetRequiredService<ILogger<MauiApp>>();

        TestLoggerErrorsSent testLoggerErrorsSent = new(_apiKey, Handler.MauiContext.Services.GetService<ILogger<MainPage>>());
        testLoggerErrorsSent.RunAllTests();
    }
}

