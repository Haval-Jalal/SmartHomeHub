namespace SmartHomeHub.Observer;

public interface ISmartHomeSubject
{
    void Subscribe(ISmartHomeObserver observer);
    void Unsubscribe(ISmartHomeObserver observer);
    void NotifyObservers(string eventType, string details);
}
