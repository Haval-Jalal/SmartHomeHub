using SmartHomeHub.Devices;
using SmartHomeHub.Observer;

namespace SmartHomeHub.Factory;

/// <summary>
/// Factory Method (Bonus) – skapar rätt IDevice-instans baserat på en string-typ,
/// och kopplar automatiskt observers. Håller skapande-logiken på ett ställe
/// så att resten av koden inte behöver känna till konkreta klasser.
/// </summary>
public static class DeviceFactory
{
    public static IDevice Create(
        string deviceType,
        string name,
        IEnumerable<ISmartHomeObserver> observers)
    {
        BaseDevice device = deviceType.ToLower() switch
        {
            "lamp"       => new Lamp(name),
            "thermostat" => new Thermostat(name),
            "doorlock"   => new DoorLock(name),
            _ => throw new ArgumentException($"Unknown device type: '{deviceType}'. Valid: lamp, thermostat, doorlock")
        };

        foreach (var obs in observers)
            device.Subscribe(obs);

        return device;
    }
}
