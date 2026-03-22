using SmartHomeHub.Singleton;

namespace SmartHomeHub.Observer;

/// <summary>
/// Observer 2 – loggar alla device-händelser via den delade Singleton-loggern.
/// </summary>
public class EventLogger : ISmartHomeObserver
{
    private readonly SmartHomeLogger _logger = SmartHomeLogger.Instance;

    public string ObserverName => "EventLogger";

    public void OnDeviceStateChanged(string deviceName, string eventType, string details)
    {
        _logger.Log($"[EVENT] Device={deviceName}, Type={eventType}, Info={details}");
    }
}
