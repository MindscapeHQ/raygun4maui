﻿namespace Raygun4Maui.SampleApp;

using Microsoft.Extensions.Configuration;
using Mindscape.Raygun4Maui;

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

        testManualExceptionsSent.runAllTests();
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

