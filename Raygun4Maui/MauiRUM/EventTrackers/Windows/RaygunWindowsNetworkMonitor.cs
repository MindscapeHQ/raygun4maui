using System.Diagnostics;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers.Windows;

public class RaygunWindowsNetworkMonitor : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, DateTime>
        _requestStartTimes =
            new Dictionary<string, DateTime>(); // TODO: Possible memory leak if there is no RequestEnd event
    
    
    private const string Request = "Request";
    private const string RequestStart = "System.Net.Http.HttpRequestOut.Start";
    private const string RequestEnd = "System.Net.Http.HttpRequestOut.Stop";
    private const string RaygunId = "X-Raygun-Request-ID";
    
    
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
                if (value.Value?.GetType().GetProperty(Request)?.GetValue(value.Value) is not HttpRequestMessage
                    request)
                {
                    return;
                }
                
                // Check if X-Request-ID already exists
                if (!request.Headers.Contains(RaygunId))
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
                
                RaygunAppEventPublisher.Publish(new NetworkRequestFinished()
                {
                    Url = request.RequestUri?.Host ?? "Unknown",
                    Method = request.Method.Method,
                    Duration = duration.Milliseconds,
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
}