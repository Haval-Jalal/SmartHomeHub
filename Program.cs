using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Facade;
using SmartHomeHub.Singleton;
using SmartHomeHub.Strategies;

// ============================================================
//  Smart Home Hub – Demo Program
//  Alla 5 designmönster + 1 bonus visas steg för steg.
// ============================================================

var hub = new SmartHomeFacade();

PrintHeader("SMART HOME CONTROL CENTER");

// ── 1. FACADE + FACTORY: lägg till enheter via Facade API ────────────
PrintSection("1. Adding devices via Facade (uses Factory internally)");

hub.AddDevice("lamp",       "LivingRoomLamp");
hub.AddDevice("lamp",       "BedroomLamp");
hub.AddDevice("thermostat", "MainThermostat");
hub.AddDevice("doorlock",   "FrontDoor");

hub.PrintDeviceStatus();

// ── 2. OBSERVER: visa att 3 observers reagerar på ett event ──────────
PrintSection("2. Observer – TurnOn triggers Dashboard + Logger + AuditLog");

var lamp = (Lamp)hub.GetDevice("LivingRoomLamp");
hub.RunCommand(new TurnOnCommand(lamp, new NormalMode()));

// ── 3. COMMAND: kö, exekvera, undo, replay ───────────────────────────
PrintSection("3. Command – Queue, Execute, Undo, Replay");

var thermostat = (Thermostat)hub.GetDevice("MainThermostat");
var door       = (DoorLock)hub.GetDevice("FrontDoor");

hub.QueueCommand(new SetTemperatureCommand(thermostat, 23.0, new NormalMode()));
hub.QueueCommand(new LockDoorCommand(door));
hub.QueueCommand(new TurnOnCommand((Lamp)hub.GetDevice("BedroomLamp"), new NormalMode()));

Console.WriteLine("\n  Executing all queued commands:");
hub.ExecuteQueue();

Console.WriteLine("\n  Undoing last command:");
hub.Undo();

Console.WriteLine("\n  Replaying last 3 commands from history:");
hub.Replay(3);

hub.PrintCommandHistory();

// ── 4. STRATEGY: byt läge och visa hur det påverkar beteendet ────────
PrintSection("4. Strategy – EcoMode blocks temperature above 21°C");

hub.SetMode(new EcoMode());
hub.RunCommand(new SetTemperatureCommand(thermostat, 25.0, new EcoMode())); // skall blockeras
hub.RunCommand(new SetTemperatureCommand(thermostat, 20.0, new EcoMode())); // skall tillåtas

PrintSection("4b. Strategy – EcoMode limits PartyRoutine to 2 devices");
hub.PartyRoutine(); // bara 2 lampor tillåts i Eco

PrintSection("4c. Strategy – PartyMode allows everything");
hub.SetMode(new PartyMode());
hub.PartyRoutine(); // alla lampor tänds

// ── 5. FACADE: MorningRoutine i ett enda anrop ───────────────────────
PrintSection("5. Facade – MorningRoutine() controls everything in one call");

hub.SetMode(new NormalMode());
hub.MorningRoutine();

hub.PrintDeviceStatus();

// ── 6. SINGLETON: visa att Logger är samma instans överallt ──────────
PrintSection("6. Singleton – Same Logger instance everywhere");

var loggerA = SmartHomeLogger.Instance;
var loggerB = SmartHomeLogger.Instance;

Console.WriteLine($"  loggerA == loggerB: {SmartHomeLogger.IsSameInstance(loggerA, loggerB)}");
Console.WriteLine($"  Total log entries so far: {loggerA.EntryCount}");

loggerA.PrintFullLog();

// ── 7. AUDIT REPORT ──────────────────────────────────────────────────
hub.AuditLog.PrintAuditReport();

// ── Helpers ──────────────────────────────────────────────────────────
static void PrintHeader(string title)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"\n{'='.ToString().PadRight(60, '=')}");
    Console.WriteLine($"  {title}");
    Console.WriteLine($"{'='.ToString().PadRight(60, '=')}");
    Console.ResetColor();
}

static void PrintSection(string title)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"\n--- {title} ---");
    Console.ResetColor();
}
