namespace SmartHomeHub.Singleton;

/// <summary>
/// Singleton – en och samma logger-instans delas av hela systemet.
/// Garanterar att alla loggposter hamnar på ett ställe, oavsett varifrån de skickas.
/// </summary>
public sealed class SmartHomeLogger
{
    private static SmartHomeLogger? _instance;
    private static readonly object _lock = new();

    private readonly List<string> _logEntries = new();

    // Privat konstruktor – ingen annan kan skapa en instans.
    private SmartHomeLogger() { }

    public static SmartHomeLogger Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (_lock)
                {
                    _instance ??= new SmartHomeLogger();
                }
            }
            return _instance;
        }
    }

    public void Log(string message)
    {
        string timestamped = $"[{DateTime.Now:HH:mm:ss}] {message}";
        _logEntries.Add(timestamped);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  [LOG]       📝 {message}");
        Console.ResetColor();
    }

    public void PrintFullLog()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n=== FULL SYSTEM LOG ===");
        foreach (var entry in _logEntries)
            Console.WriteLine($"  {entry}");
        Console.WriteLine("=======================");
        Console.ResetColor();
    }

    public int EntryCount => _logEntries.Count;

    // Returnerar referens till instansen för att demonstrera Singleton (samma objekt)
    public static bool IsSameInstance(SmartHomeLogger a, SmartHomeLogger b) =>
        ReferenceEquals(a, b);
}
