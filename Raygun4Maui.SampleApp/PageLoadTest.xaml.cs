using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raygun4Maui.SampleApp;

public partial class PageLoadTest : ContentPage
{
    public PageLoadTest()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await Task.Delay(1500);
    }

    private async void OnNavigateButtonClicked(object sender, EventArgs e)
    {
        var responseMessage = await new HttpClient().GetAsync("https://www.google.com/");
        await Navigation.PopAsync();
    }
}