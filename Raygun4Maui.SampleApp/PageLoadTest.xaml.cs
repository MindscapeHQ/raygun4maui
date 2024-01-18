using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;
#if ANDROID
using Java.IO;
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
        #if ANDROID
        HttpURLConnection urlConnection = null;
        try
        {
            URL javaUrl = new URL("https://www.google.com/");
            urlConnection = (HttpURLConnection)javaUrl.OpenConnection();

        // using (var reader = new BufferedReader(new InputStreamReader(urlConnection.InputStream)))
        //     {
        //         StringBuilder total = new StringBuilder();
        //         string line;
        //         while ((line = reader.ReadLine()) != null)
        //         {
        //             total.Append(line).Append('\n');
        //         }
        //         // Console.WriteLine(total.ToString());
        //     }
        }
        catch (Java.IO.IOException ex)
        {
            await RaygunMauiClient.Current.SendInBackground(ex);
            // Handle the exception
            // Console.WriteLine(ex);
        }
        finally
        {
            urlConnection?.Disconnect();
        }
        #endif
        
        // var responseMessage = await new HttpClient().GetAsync("https://www.google.com/");
        await Navigation.PopAsync();
    }

    
}