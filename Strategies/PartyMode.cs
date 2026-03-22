namespace SmartHomeHub.Strategies;

/// <summary>
/// PartyMode – inga begränsningar och extra hög temperaturtolerens.
/// Facade använder detta läge för batch-operationer (alla lampor på).
/// </summary>
public class PartyMode : IModeStrategy
{
    public string ModeName => "Party";
    public int MaxSimultaneousDevices => int.MaxValue;

    public bool CanTurnOnDevice(string deviceName) => true;

    public bool CanChangeTemperature(double currentTemp, double targetTemp)
    {
        // Partyläge tillåter upp till 28°C
        if (targetTemp > 28.0)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"  [PARTY MODE] ❌ Even parties have limits! Max 28°C.");
            Console.ResetColor();
            return false;
        }
        return true;
    }

    public string GetModeDescription() =>
        "Party mode: All devices allowed. Temperature up to 28°C. Batch lamp control enabled!";
}
