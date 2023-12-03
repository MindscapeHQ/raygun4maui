using System.Net;

namespace Raygun4Maui.MauiRUM;

public class RaygunWebRequestHandler : IRaygunWebRequestHandler
  {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly Uri _endpoint;
    
    public RaygunWebRequestHandler(string apiKey, Uri endpoint, int timeout)
    {
      _apiKey = apiKey;
      _endpoint = endpoint;

      _httpClient = new HttpClient();
      _httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
    }

    public bool IsOnline()
    {
      return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    public int Post(string payload)
    {
      int statusCode = 0;

      try
      {
        // Create the request contnet.
        HttpContent content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

        // Add API key to headers.
        content.Headers.Add("X-ApiKey", _apiKey);

        // Perform the request.
        var task = _httpClient.PostAsync(_endpoint, content);

        task.Wait();

        var response = task.Result;

        // Check the response.
        statusCode = (int)response.StatusCode;
      }
      catch (Exception e)
      {
        // RaygunLogger.Error($"Failed to send message due to error {e?.GetType()?.Name}: {e?.Message}");

        // Was this due to the request timing out?
        const string taskCanceledEx = "TaskCanceledException";


        if (e?.GetType()?.Name == taskCanceledEx || e?.InnerException?.GetType()?.Name == taskCanceledEx)
        {
          statusCode = (int)HttpStatusCode.RequestTimeout;
        }
        else
        {
          statusCode = (int)HttpStatusCode.BadRequest;
        }
      }

      return statusCode;
    }

    public async Task<int> PostAsync(string payload)
    {
      int statusCode = 0;

      try
      {
        // Create the request contnet.
        HttpContent content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

        // Add API key to headers.
        content.Headers.Add("X-ApiKey", _apiKey);

        // Perform the request.
        var response = await _httpClient.PostAsync(_endpoint, content);

        // Check the response.
        statusCode = (int)response.StatusCode;
      }
      catch (Exception e)
      {
        // RaygunLogger.Error($"Failed to send message due to error {e.GetType().Name}: {e.Message}");

        // Was this due to the request timing out?
        const string taskCanceledEx = "TaskCanceledException";
        // Null reference exception will be thrown if the InnerException is null. coalesce the lookup
        if (e?.GetType()?.Name == taskCanceledEx || e?.InnerException?.GetType()?.Name == taskCanceledEx)
        {
          statusCode = (int)HttpStatusCode.RequestTimeout;
        }
        else
        {
          statusCode = (int)HttpStatusCode.BadRequest;
        }
      }

      return statusCode;
    }
  }