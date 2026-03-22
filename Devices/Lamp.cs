namespace SmartHomeHub.Devices;

public class Lamp : BaseDevice
{
    public int Brightness { get; private set; }

    public Lamp(string name) : base(name)
    {
        Brightness = 100;
    }

    public void SetBrightness(int level)
    {
        Brightness = Math.Clamp(level, 0, 100);
        NotifyObservers("BrightnessChange", $"{Name} brightness set to {Brightness}%");
    }

    public override string GetStatus()
        => $"[Lamp] {Name}: {(IsOn ? "ON" : "OFF")}, Brightness: {Brightness}%";
}
