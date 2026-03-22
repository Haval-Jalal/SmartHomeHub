namespace SmartHomeHub.Strategies;

public interface IModeStrategy
{
    string ModeName { get; }
    bool CanTurnOnDevice(string deviceName);
    bool CanChangeTemperature(double currentTemp, double targetTemp);
    int MaxSimultaneousDevices { get; }
    string GetModeDescription();
}
