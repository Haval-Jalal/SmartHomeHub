namespace SmartHomeHub.Observer;

/// <summary>
/// Observer 3 – håller en intern lista med säkerhetsrelaterade händelser (audit trail).
/// Reagerar enbart på kritiska event-typer som lås och larm.
/// </summary>
public class AuditLog : ISmartHomeObserver
{
    private readonly List<string> _auditEntries = new();
    private static readonly HashSet<string> CriticalEvents =
        new(StringComparer.OrdinalIgnoreCase) { "Locked", "Unlocked", "TurnOn", "TurnOff" };

    public string ObserverName => "AuditLog";

    public void OnDeviceStateChanged(string deviceName, string eventType, string details)
    {
        if (!CriticalEvents.Contains(eventType)) return;

        string entry = $"[{DateTime.Now:HH:mm:ss}] AUDIT | {details}";
        _auditEntries.Add(entry);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  [AUDIT]     🔐 {details}");
        Console.ResetColor();
    }

    public void PrintAuditReport()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n=== AUDIT REPORT ===");
        if (_auditEntries.Count == 0)
        {
            Console.WriteLine("  (No audit entries)");
        }
        else
        {
            foreach (var entry in _auditEntries)
                Console.WriteLine($"  {entry}");
        }
        Console.WriteLine("====================");
        Console.ResetColor();
    }
}
