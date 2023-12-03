namespace Raygun4Maui.AppEvents;

public class RaygunAppEventPublisher
{
    public static RaygunAppEventPublisher Instance 
    {
        get 
        {
            if (_publisher == null)
            {
                _publisher = new RaygunAppEventPublisher();
            }

            return _publisher;
        } 
    }

    private static RaygunAppEventPublisher _publisher;
    private Dictionary<RaygunAppEventType, List<Action<IRaygunAppEventArgs>>> _listeners; // TODO: Maybe make an alias for this like Xamarin?

    private RaygunAppEventPublisher()
    {
        _listeners = new Dictionary<RaygunAppEventType, List<Action<IRaygunAppEventArgs>>>();
    }

    public static void ListenFor(RaygunAppEventType eventType, Action<IRaygunAppEventArgs> callback)
    {
        System.Diagnostics.Debug.WriteLine("RAHHHHHHHHHHHHHHHHHHHHH");
        System.Diagnostics.Debug.WriteLine($"Callback added {callback.ToString()}");

        Instance.AddListener(eventType, callback);
    }

    private void AddListener(RaygunAppEventType eventType, Action<IRaygunAppEventArgs> callback)
    {
        if (HasRegisteredEventOf(eventType))
        {
            _listeners[eventType].Add(callback);
        }
        else
        {
            _listeners.Add(eventType, new List<Action<IRaygunAppEventArgs>>() { callback });
        }
    }

    public static void EventOccurred(RaygunAppEventType type)
    {
        EventOccurred(type, new RaygunAppEventArgs(type));
    }

    public static void EventOccurred(RaygunAppEventType type, IRaygunAppEventArgs args)
    {
        Instance.NotifyListeners(args);
    }

    private void NotifyListeners(IRaygunAppEventArgs args)
    {
        System.Diagnostics.Debug.WriteLine($"NEW EVENT!!!!!!!!!!!!! {args.Type}");
        
        if (HasRegisteredEventOf(args.Type))
        {
            var callbacks = _listeners[args.Type];

            foreach (var callback in callbacks)
            {
                callback?.Invoke(args);
            }
        }
    }

    private bool HasRegisteredEventOf(RaygunAppEventType eventType)
    {
        return _listeners.ContainsKey(eventType);
    }
}