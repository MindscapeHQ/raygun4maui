#if ANDROID
using Java.Net;
#endif

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
        // Android networking library only works for non HttpClient conenctions as they do not call .disconnect on the HttpURLConnection
#if ANDROID
        HttpURLConnection urlConnection = null;
        
        // Must be in a seperate thread as we will get a NetworkOnMainThreadException otherwise
        await Task.Run(() =>
        {
            try
            {
                URL javaUrl = new URL("https://www.google.com/");
                urlConnection = (HttpURLConnection)javaUrl.OpenConnection();
            }
            catch (Java.IO.IOException ex)
            {
                RaygunMauiClient.Current.SendInBackground(ex);
            }
            finally
            {
                urlConnection?.Disconnect();
            }
        });
#else
        var responseMessage = await new HttpClient().GetAsync("https://www.google.com/");
#endif
        await Navigation.PopAsync();
    }
}