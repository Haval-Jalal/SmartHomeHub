using SmartHomeHub.Observer;

namespace SmartHomeHub.Devices;

public abstract class BaseDevice : IDevice, ISmartHomeSubject
{
    private readonly List<ISmartHomeObserver> _observers = new();

    public string Name { get; }
    public bool IsOn { get; protected set; }

    protected BaseDevice(string name)
    {
        Name = name;
        IsOn = false;
    }

    public virtual void TurnOn()
    {
        IsOn = true;
        NotifyObservers("TurnOn", $"{Name} turned ON");
    }

    public virtual void TurnOff()
    {
        IsOn = false;
        NotifyObservers("TurnOff", $"{Name} turned OFF");
    }

    public abstract string GetStatus();

    public void Subscribe(ISmartHomeObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(ISmartHomeObserver observer)
    {
        _observers.Remove(observer);
    }

    public void NotifyObservers(string eventType, string details)
    {
        foreach (var observer in _observers)
            observer.OnDeviceStateChanged(Name, eventType, details);
    }
}
