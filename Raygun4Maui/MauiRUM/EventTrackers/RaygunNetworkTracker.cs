using System.Diagnostics;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunNetworkTracker : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, DateTime>
        _requestStartTimes =
            new Dictionary<string, DateTime>(); // TODO: Possible memory leak if there is no RequestEnd event

    private Raygun4MauiSettings _settings;

    public event Action<RaygunTimingEventArgs> NetworkRequestCompleted;


    // private const string Request = "Request";
    private const string RequestStart = "System.Net.Http.HttpRequestOut.Start";
    private const string RequestEnd = "System.Net.Http.HttpRequestOut.Stop";
    private const string RaygunId = "X-Raygun-Request-ID";

    public void Init(Raygun4MauiSettings settings)
    {
        _settings = settings;
    }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "HttpHandlerDiagnosticListener")
        {
            listener.Subscribe(this);
        }
    }

    public void OnNext(KeyValuePair<string, object> value)
    {
        switch (value.Key)
        {
            case RequestStart:
            {
                if (value.Value?.GetType().GetProperty("Request")?.GetValue(value.Value) is not HttpRequestMessage
                    request)
                {
                    return;
                }

                // TODO: Make this a function to check proxy and raygun
                if (request.RequestUri?.Host.Contains("api.raygun.com") ??
                    true) // Raygun4Net can set proxy, also do a test case for the RaygunId
                {
                    return;
                }

                // Check if X-Request-ID already exists
                if (!request.Headers.Contains(RaygunId)) // Constant (Raygun)
                {
                    var trackingId = Guid.NewGuid().ToString();
                    request.Headers.Add(RaygunId, trackingId);
                    _requestStartTimes[trackingId] = DateTime.UtcNow;
                }
                else
                {
                    // Use existing X-Request-ID
                    var existingTrackingId = request.Headers.GetValues(RaygunId).FirstOrDefault();
                    if (existingTrackingId != null)
                    {
                        _requestStartTimes[existingTrackingId] =
                            DateTime.UtcNow; // TODO: Inject DateTime as a service to unit test
                    }
                }

                break;
            }
            case RequestEnd:
            {
                if (value.Value?.GetType().GetProperty("Request")?.GetValue(value.Value) is not HttpRequestMessage
                    request)
                {
                    return;
                }

                // TODO: Fix ordering
                if (request.RequestUri?.Host.Contains("api.raygun.com") ?? true)
                {
                    return;
                }

                if (!request.Headers.TryGetValues(RaygunId, out var values))
                {
                    return;
                }

                var trackingId = values.FirstOrDefault();

                if (trackingId == null) return;
                if (!_requestStartTimes.TryGetValue(trackingId, out var startTime))
                {
                    return;
                }

                var endTime = DateTime.UtcNow;
                var duration = (endTime - startTime);

                _requestStartTimes.Remove(trackingId);

                NetworkRequestCompleted?.Invoke(new RaygunTimingEventArgs()
                {
                    Type = RaygunRumEventTimingType.NetworkCall,
                    Key = $"{request.Method} {request.RequestUri}",
                    Milliseconds = duration.Milliseconds
                });

                break;
            }
        }
    }

    public void OnError(Exception error)
    {
        // Handle any errors here
    }

    public void OnCompleted()
    {
        // Handle completion here
    }

    public bool ShouldIgnore(string url)
    {
        return _settings.IgnoredViews != null && _settings.IgnoredUrls.Contains(url);
    }
}