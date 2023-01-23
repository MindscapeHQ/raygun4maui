namespace Raygun4Maui.SampleApp;

using Microsoft.Extensions.Configuration;
using raygun4maui;

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
            apiKeyLabel.Text += _apiKey;
        }
        else
        {
            apiKeyLabel.Text += "Not set! Please set it Right-Click on the project solution and selecting Manage User Secrets";
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

        RaygunMauiClient raygunMauiClient = new(_apiKey);

        raygunMauiClient.SendInBackground(new Exception("Raygun4Maui.SampleApp Manual Exception @ " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
    }

    private void OnUnhandledExceptionClicked(object sender, EventArgs e)
    {
        UnhandledExceptionButton.Text += ".";
    }

    private void OnILoggerErrorClicked(object sender, EventArgs e)
    {
        ILoggerButton.Text += ".";
    }
}

