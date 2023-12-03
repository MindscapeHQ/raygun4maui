using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunNetworkTracker : EventListener
{
    private Dictionary<int, long> _requestStore = new Dictionary<int, long>();

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == "System.Net.Http")
        {
            EnableEvents(eventSource, EventLevel.Informational);
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        // eventData.
        // switch (eventData.EventName)
        // {
        //     case "RequestStart":
        //         _requestStore.Add(eventData.EventId);
        //         break;
        //     case "RequestStop":
        //         break;
        // }

        // eventData.EventName
        System.Diagnostics.Debug.WriteLine($"{DateTime.UtcNow:ss:fff} {eventData.EventName} ({string.Join(' ', eventData.PayloadNames )}): " +
                                           string.Join(' ',
                                               eventData.PayloadNames!.Zip(eventData.Payload!)
                                                   .Select(pair => $"{pair.First}={pair.Second}")));
    }
}