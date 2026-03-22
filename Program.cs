using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Facade;
using SmartHomeHub.Singleton;
using SmartHomeHub.Strategies;

// ── Setup ─────────────────────────────────────────────────────────────────────
var hub = new SmartHomeFacade();

hub.AddDevice("lamp",       "LivingRoomLamp");
hub.AddDevice("lamp",       "BedroomLamp");
hub.AddDevice("thermostat", "MainThermostat");
hub.AddDevice("doorlock",   "FrontDoor");

var lamp       = (Lamp)hub.GetDevice("LivingRoomLamp");
var bedroom    = (Lamp)hub.GetDevice("BedroomLamp");
var thermostat = (Thermostat)hub.GetDevice("MainThermostat");
var door       = (DoorLock)hub.GetDevice("FrontDoor");

// ── Main loop ─────────────────────────────────────────────────────────────────
bool running = true;
while (running)
{
    PrintHeader();
    Console.WriteLine("  Vad vill du göra?\n");
    Console.WriteLine("  --- ENHETER ---");
    Console.WriteLine("  [1] Tänd LivingRoomLamp");
    Console.WriteLine("  [2] Släck LivingRoomLamp");
    Console.WriteLine("  [3] Tänd BedroomLamp");
    Console.WriteLine("  [4] Släck BedroomLamp");
    Console.WriteLine("  [5] Sätt temperatur (du väljer)");
    Console.WriteLine("  [6] Lås FrontDoor");
    Console.WriteLine("  [7] Lås upp FrontDoor");
    Console.WriteLine();
    Console.WriteLine("  --- LÄGEN (Strategy) ---");
    Console.WriteLine("  [8]  Byt till NormalMode");
    Console.WriteLine("  [9]  Byt till EcoMode  (max 21°C, max 2 enheter)");
    Console.WriteLine("  [10] Byt till PartyMode (max 28°C, alla lampor)");
    Console.WriteLine();
    Console.WriteLine("  --- RUTINER (Facade) ---");
    Console.WriteLine("  [11] MorningRoutine");
    Console.WriteLine("  [12] PartyRoutine");
    Console.WriteLine();
    Console.WriteLine("  --- COMMAND-HISTORIK ---");
    Console.WriteLine("  [13] Ångra senaste kommando (Undo)");
    Console.WriteLine("  [14] Replay – kör om senaste 5");
    Console.WriteLine("  [15] Visa kommandohistorik");
    Console.WriteLine();
    Console.WriteLine("  --- STATUS & LOGGAR ---");
    Console.WriteLine("  [16] Visa enhetsstatus");
    Console.WriteLine("  [17] Visa full systemlogg (Singleton)");
    Console.WriteLine("  [18] Visa audit-rapport");
    Console.WriteLine("  [19] Bevisa att Logger är samma instans (Singleton)");
    Console.WriteLine();
    Console.WriteLine("  [0]  Avsluta");
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write($"  Nuvarande läge: {hub.CurrentMode}  > Välj: ");
    Console.ResetColor();

    var input = Console.ReadLine()?.Trim();
    Console.WriteLine();

    switch (input)
    {
        case "1":
            hub.RunCommand(new TurnOnCommand(lamp, GetMode()));
            break;
        case "2":
            hub.RunCommand(new TurnOffCommand(lamp));
            break;
        case "3":
            hub.RunCommand(new TurnOnCommand(bedroom, GetMode()));
            break;
        case "4":
            hub.RunCommand(new TurnOffCommand(bedroom));
            break;
        case "5":
            Console.Write("  Ange temperatur (°C): ");
            if (double.TryParse(Console.ReadLine(), out double temp))
                hub.RunCommand(new SetTemperatureCommand(thermostat, temp, GetMode()));
            else
                Msg("Ogiltigt värde.");
            break;
        case "6":
            hub.RunCommand(new LockDoorCommand(door));
            break;
        case "7":
            // Unlock = TurnOn på DoorLock
            hub.RunCommand(new TurnOnCommand(door, GetMode()));
            break;
        case "8":
            hub.SetMode(new NormalMode());
            break;
        case "9":
            hub.SetMode(new EcoMode());
            break;
        case "10":
            hub.SetMode(new PartyMode());
            break;
        case "11":
            hub.MorningRoutine();
            break;
        case "12":
            hub.PartyRoutine();
            break;
        case "13":
            hub.Undo();
            break;
        case "14":
            hub.Replay(5);
            break;
        case "15":
            hub.PrintCommandHistory();
            break;
        case "16":
            hub.PrintDeviceStatus();
            break;
        case "17":
            SmartHomeLogger.Instance.PrintFullLog();
            break;
        case "18":
            hub.AuditLog.PrintAuditReport();
            break;
        case "19":
            var a = SmartHomeLogger.Instance;
            var b = SmartHomeLogger.Instance;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"  loggerA.GetHashCode() = {a.GetHashCode()}");
            Console.WriteLine($"  loggerB.GetHashCode() = {b.GetHashCode()}");
            Console.WriteLine($"  Samma instans?        = {SmartHomeLogger.IsSameInstance(a, b)}");
            Console.ResetColor();
            break;
        case "0":
            running = false;
            Console.WriteLine("  Avslutar Smart Home Hub. Hej då!");
            break;
        default:
            Msg("Okänt val, försök igen.");
            break;
    }

    if (running)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\n  Tryck Enter för att fortsätta...");
        Console.ResetColor();
        Console.ReadLine();
        Console.Clear();
    }
}

// ── Helpers ───────────────────────────────────────────────────────────────────
IModeStrategy GetMode()
{
    return hub.CurrentMode switch
    {
        "Eco"   => new EcoMode(),
        "Party" => new PartyMode(),
        _       => new NormalMode()
    };
}

void PrintHeader()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════╗");
    Console.WriteLine("║         🏠  SMART HOME CONTROL CENTER  🏠          ║");
    Console.WriteLine("╚════════════════════════════════════════════════════╝");
    Console.ResetColor();
}

void Msg(string text)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  {text}");
    Console.ResetColor();
}
