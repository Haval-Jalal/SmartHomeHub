namespace SmartHomeHub.Strategies;

/// <summary>
/// NormalMode – inga restriktioner, allt funkar som vanligt.
/// </summary>
public class NormalMode : IModeStrategy
{
    public string ModeName => "Normal";
    public int MaxSimultaneousDevices => int.MaxValue;

    public bool CanTurnOnDevice(string deviceName) => true;

    public bool CanChangeTemperature(double currentTemp, double targetTemp) => true;

    public string GetModeDescription() =>
        "Normal mode: No restrictions. All devices can be controlled freely.";
}
