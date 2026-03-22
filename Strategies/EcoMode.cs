namespace SmartHomeHub.Strategies;

/// <summary>
/// EcoMode – energisparläge. Begränsar temperaturändringar och max antal aktiva enheter.
/// Strategy påverkar both command-validering och facade-logik.
/// </summary>
public class EcoMode : IModeStrategy
{
    private const double MaxTemp = 21.0;
    private const double MinTemp = 18.0;

    public string ModeName => "Eco";
    public int MaxSimultaneousDevices => 2;

    public bool CanTurnOnDevice(string deviceName) => true; // tillåts men begränsas via MaxSimultaneousDevices

    public bool CanChangeTemperature(double currentTemp, double targetTemp)
    {
        if (targetTemp > MaxTemp)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [ECO MODE]  ❌ Temperature {targetTemp}°C exceeds eco limit ({MaxTemp}°C).");
            Console.ResetColor();
            return false;
        }
        if (targetTemp < MinTemp)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [ECO MODE]  ❌ Temperature {targetTemp}°C is below eco minimum ({MinTemp}°C).");
            Console.ResetColor();
            return false;
        }
        return true;
    }

    public string GetModeDescription() =>
        $"Eco mode: Max {MaxSimultaneousDevices} devices ON. Temperature locked between {MinTemp}–{MaxTemp}°C.";
}
