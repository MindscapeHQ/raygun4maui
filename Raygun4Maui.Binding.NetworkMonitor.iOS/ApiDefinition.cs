using Foundation;

namespace Raygun4Maui.Binding.NetworkMonitor.iOS
{
  [BaseType(typeof(NSObject))]
  public interface RaygunNetworkMonitor
  {
    [Static, Export("RequestOccurredNotificationName")]
    NSString RequestOccurredNotificationName { get; }

    [Static, Export("RequestUrlNotificationKey")]
    NSString RequestUrlNotificationKey { get; }

    [Static, Export("RequestMethodNotificationKey")]
    NSString RequestMethodNotificationKey { get; }

    [Static, Export("RequestDurationNotificationKey")]
    NSString RequestDurationNotificationKey { get; }

    [Static, Export("enable")]
    void Enable();
  }
}

