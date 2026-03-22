namespace SmartHomeHub.Devices;

public class DoorLock : BaseDevice
{
    public bool IsLocked { get; private set; }

    public DoorLock(string name) : base(name)
    {
        IsLocked = true;
    }

    public void Lock()
    {
        IsLocked = true;
        NotifyObservers("Locked", $"{Name} is now LOCKED");
    }

    public void Unlock()
    {
        IsLocked = false;
        NotifyObservers("Unlocked", $"{Name} is now UNLOCKED");
    }

    public override void TurnOn() => Unlock();
    public override void TurnOff() => Lock();

    public override string GetStatus()
        => $"[DoorLock] {Name}: {(IsLocked ? "LOCKED" : "UNLOCKED")}";
}
