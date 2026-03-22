namespace SmartHomeHub.Devices;

public class Thermostat : BaseDevice
{
    public double CurrentTemperature { get; private set; }
    public double TargetTemperature { get; private set; }

    public Thermostat(string name, double initialTemp = 20.0) : base(name)
    {
        CurrentTemperature = initialTemp;
        TargetTemperature = initialTemp;
    }

    public void SetTemperature(double target)
    {
        double previous = TargetTemperature;
        TargetTemperature = target;
        NotifyObservers("TemperatureChange",
            $"{Name} temperature changed from {previous}°C to {TargetTemperature}°C");
    }

    public override string GetStatus()
        => $"[Thermostat] {Name}: {(IsOn ? "ON" : "OFF")}, Current: {CurrentTemperature}°C, Target: {TargetTemperature}°C";
}
