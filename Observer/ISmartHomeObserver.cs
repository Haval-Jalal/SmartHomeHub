namespace SmartHomeHub.Observer;

public interface ISmartHomeObserver
{
    string ObserverName { get; }
    void OnDeviceStateChanged(string deviceName, string eventType, string details);
}
