﻿using Serilog;

namespace Raygun4Maui.SampleApp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestingLogic;

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
            ApiKeyLabel.Text +=
                "Not set! Please set it Right-Click on the project solution and selecting Manage User Secrets";
        }
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        CounterBtn.Text = $"Clicked {count} time{(count > 1 ? "s" : string.Empty)}";

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

        TestUnhandledExceptionsSent.RunAllTests();
    }

    private void OnILoggerErrorClicked(object sender, EventArgs e)
    {
        ILoggerButton.Text += ".";

        ILogger logger = Handler!.MauiContext!.Services.GetService<ILogger<MainPage>>();

        TestLoggerErrorsSent testLoggerErrorsSent = new(_apiKey, logger);
        testLoggerErrorsSent.RunAllTests();
    }

    private void OnSerilogClicked(object sender, EventArgs e)
    {
        try
        {
            throw new ApplicationException("Captured by Serilog I hope...");
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Serilog error");
        }
    }
}