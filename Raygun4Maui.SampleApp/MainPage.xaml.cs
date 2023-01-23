namespace Raygun4Maui.SampleApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
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

