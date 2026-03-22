namespace SmartHomeHub.Devices;

public interface IDevice
{
    string Name { get; }
    bool IsOn { get; }
    void TurnOn();
    void TurnOff();
    string GetStatus();
}
