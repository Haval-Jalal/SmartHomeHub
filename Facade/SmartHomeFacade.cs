using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Factory;
using SmartHomeHub.Observer;
using SmartHomeHub.Singleton;
using SmartHomeHub.Strategies;

namespace SmartHomeHub.Facade;

/// <summary>
/// SmartHomeFacade – ger ett rent, enkelt API mot det komplexa systemet.
/// Program.cs behöver inte känna till Observer, Command, Strategy eller Factory-detaljer.
/// </summary>
public class SmartHomeFacade
{
    // --- Internt tillstånd ---
    private readonly Dictionary<string, IDevice> _devices = new();
    private readonly List<ISmartHomeObserver> _observers;
    private readonly CommandInvoker _invoker = new();
    private readonly SmartHomeLogger _logger = SmartHomeLogger.Instance;

    private IModeStrategy _currentMode;

    // Observers är publika för att kunna användas i demo (t.ex. AuditLog.PrintAuditReport)
    public Dashboard Dashboard { get; } = new();
    public EventLogger EventLogger { get; } = new();
    public AuditLog AuditLog { get; } = new();

    public SmartHomeFacade()
    {
        _observers = new List<ISmartHomeObserver> { Dashboard, EventLogger, AuditLog };
        _currentMode = new NormalMode();
        _logger.Log("SmartHomeFacade initialized.");
    }

    // ── Device Management ──────────────────────────────────────────────

    /// <summary>Skapar och registrerar en enhet via Factory.</summary>
    public IDevice AddDevice(string type, string name)
    {
        var device = DeviceFactory.Create(type, name, _observers);
        _devices[name] = device;
        _logger.Log($"Device registered: {name} ({type})");
        return device;
    }

    // ── Mode / Strategy ────────────────────────────────────────────────

    public void SetMode(IModeStrategy mode)
    {
        _currentMode = mode;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n  [HUB MODE]  🔄 Mode changed to: {mode.ModeName}");
        Console.WriteLine($"              {mode.GetModeDescription()}");
        Console.ResetColor();
        _logger.Log($"Mode set to: {mode.ModeName}");
    }

    public string CurrentMode => _currentMode.ModeName;

    // ── Command API ────────────────────────────────────────────────────

    public void RunCommand(ICommand cmd)
    {
        _invoker.Enqueue(cmd);
        _invoker.ExecuteNext();
    }

    public void QueueCommand(ICommand cmd) => _invoker.Enqueue(cmd);

    public void ExecuteQueue() => _invoker.ExecuteAll();

    public void Undo() => _invoker.Undo();

    public void Replay(int count = 5) => _invoker.Replay(count);

    public void PrintCommandHistory() => _invoker.PrintHistory();

    // ── High-Level Routines ────────────────────────────────────────────

    /// <summary>
    /// MorningRoutine – låser upp dörren, sätter temperaturen och tänder lamporna.
    /// Demonstrerar att Facade kan koordinera flera mönster i ett enda anrop.
    /// </summary>
    public void MorningRoutine()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n  ☀️  === MORNING ROUTINE START ===");
        Console.ResetColor();

        foreach (var device in _devices.Values)
        {
            var cmd = device switch
            {
                DoorLock door     => (ICommand)new LockDoorCommand(door),  // säkra dörren
                Thermostat thermo => new SetTemperatureCommand(thermo, 22.0, _currentMode),
                Lamp lamp         => new TurnOnCommand(lamp, _currentMode),
                _                 => new TurnOnCommand(device, _currentMode)
            };
            _invoker.Enqueue(cmd);
        }

        _invoker.ExecuteAll();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  ☀️  === MORNING ROUTINE DONE ===\n");
        Console.ResetColor();
    }

    /// <summary>
    /// PartyRoutine – tänder alla lampor på en gång (kräver PartyMode för full effekt).
    /// </summary>
    public void PartyRoutine()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n  🎉 === PARTY ROUTINE START ===");
        Console.ResetColor();

        int activeCount = 0;
        foreach (var device in _devices.Values.OfType<Lamp>())
        {
            if (activeCount >= _currentMode.MaxSimultaneousDevices)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  [BLOCKED]   ❌ Mode '{_currentMode.ModeName}' limits to {_currentMode.MaxSimultaneousDevices} devices. Skipping {device.Name}.");
                Console.ResetColor();
                continue;
            }
            RunCommand(new TurnOnCommand(device, _currentMode));
            activeCount++;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  🎉 === PARTY ROUTINE DONE ===\n");
        Console.ResetColor();
    }

    // ── Status ─────────────────────────────────────────────────────────

    public void PrintDeviceStatus()
    {
        Console.WriteLine("\n  === DEVICE STATUS ===");
        foreach (var d in _devices.Values)
            Console.WriteLine($"  {d.GetStatus()}");
        Console.WriteLine("  =====================\n");
    }

    public IDevice GetDevice(string name) => _devices[name];
}
