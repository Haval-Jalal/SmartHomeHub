namespace SmartHomeHub.Observer;

/// <summary>
/// Observer 1 – visar live-uppdateringar i terminalen som en dashboard.
/// </summary>
public class Dashboard : ISmartHomeObserver
{
    public string ObserverName => "Dashboard";

    public void OnDeviceStateChanged(string deviceName, string eventType, string details)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  [DASHBOARD] 📊 {details}");
        Console.ResetColor();
    }
}
