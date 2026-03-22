# Smart Home Hub 🏠

En konsolapplikation i C# (.NET 8) som simulerar ett smart hemkontrollsystem.
Projektet demonstrerar **5 designmönster** (+ 1 bonusmönster) i ett sammanhängande, körbart system.

---

## Hur man kör programmet

```bash
# Klona repot
git clone <repo-url>
cd SmartHomeHub

# Kör programmet
dotnet run
```

Kräver .NET 8 SDK installerat.

---

## Designmönster – var, varför och vilket problem de löser

### 1. Observer – live-uppdateringar (`Observer/`)

**Var:** `BaseDevice` implementerar `ISmartHomeSubject`. Tre observers: `Dashboard`, `EventLogger`, `AuditLog`.

**Problemet:** När en enhet byter tillstånd behövde flera oberoende delar av systemet reagera *automatiskt* – utan att enheten ska veta vilka.

**Lösningen:** Observer skapar en en-till-många-relation. `BaseDevice.NotifyObservers()` anropas vid varje tillståndsändring och meddelar alla registrerade lyssnare. Dashboard visar uppdateringar i realtid, EventLogger skriver till systemloggen, och AuditLog spårar säkerhetskritiska händelser (lås, strömbrytning). De tre observers har *olika ansvarsområden*, vilket uppfyller VG-kravet.

---

### 2. Command – actions som objekt (`Commands/`)

**Var:** `ICommand` med implementationerna `TurnOnCommand`, `TurnOffCommand`, `SetTemperatureCommand`, `LockDoorCommand`. `CommandInvoker` hanterar kö, historik, replay och undo.

**Problemet:** Vi behövde kunna köa kommandon, spela upp dem igen och ångra dem – utan att koppla avsändaren direkt till mottagaren.

**Lösningen:** Command kapslar in varje begäran som ett objekt med `Execute()` och `Undo()`. `CommandInvoker` sparar historik i en `Stack<ICommand>` för undo, och `Replay(n)` kör de senaste N kommandona om igen. Detta uppfyller VG-kravet om *replay och undo light*.

---

### 3. Strategy – utbytbara lägen (`Strategies/`)

**Var:** `IModeStrategy` med tre implementationer: `NormalMode`, `EcoMode`, `PartyMode`.

**Problemet:** Systemets beteenderegler (vad som är tillåtet) behövde kunna bytas ut vid körning utan att skriva om befintlig kod.

**Lösningen:** Strategy kapslar in algoritmerna (tillståndsregler) bakom ett gemensamt gränssnitt. `EcoMode` begränsar temperaturändringar och max antal aktiva enheter. `PartyMode` tillåter batch-kontroll av alla lampor. Strategin injiceras i kommandon *och* används av Facade vid rutiner – den påverkar alltså **flera delar** av systemet (VG-krav), inte bara en enda if-sats.

---

### 4. Facade – rent hub-API (`Facade/SmartHomeFacade.cs`)

**Var:** `SmartHomeFacade` exponerar: `AddDevice()`, `SetMode()`, `RunCommand()`, `QueueCommand()`, `ExecuteQueue()`, `Undo()`, `Replay()`, `MorningRoutine()`, `PartyRoutine()`, `PrintDeviceStatus()`.

**Problemet:** `Program.cs` skulle inte behöva känna till Observer-prenumerationer, Factory-logik, CommandInvoker-detaljer eller hur Strategy kopplas ihop – det skapar onödig koppling.

**Lösningen:** Facade ger ett förenklat gränssnitt till ett komplext delsystem. Hela `Program.cs` styrs nästan uteslutande via `hub.*`-anrop. Det känns som ett "riktigt API" (VG-krav) – intern implementationsdetalj är dold.

---

### 5. Singleton – delad logger (`Singleton/SmartHomeLogger.cs`)

**Var:** `SmartHomeLogger.Instance` används av `CommandInvoker`, `EventLogger` och `SmartHomeFacade`.

**Problemet:** Systemet behöver en enda, gemensam logg som samlar alla händelser oavsett varifrån de rapporteras.

**Lösningen:** Singleton (thread-safe med double-check locking) garanterar att det bara finns en `SmartHomeLogger`-instans. Demonstrationen i `Program.cs` visar explicit att `loggerA == loggerB` är `True` trots att de hämtas från olika platser i koden.

---

### Bonus: Factory Method (`Factory/DeviceFactory.cs`)

**Var:** `DeviceFactory.Create(type, name, observers)` anropas av `SmartHomeFacade.AddDevice()`.

**Problemet:** Facade behövde skapa olika enhetstyper (Lamp, Thermostat, DoorLock) utan att ha `new`-anrop och observer-koppling utspridda överallt.

**Lösningen:** Factory Method centraliserar skapandelogiken. Man skickar in en string (`"lamp"`, `"thermostat"`, `"doorlock"`) och får tillbaka en korrekt konfigurerad `IDevice` med observers redan kopplade. Lätt att utöka med nya enhetstyper på ett ställe.

---

## Reflektion – när ska man INTE använda mönster?

Designmönster är verktyg, inte mål i sig. Några situationer där de gör mer skada än nytta:

- **Observer** är onödigt om det bara finns *en* mottagare. Då räcker ett direkt metodanrop – Observer tillför komplexitet utan vinst.
- **Command** är överdrivet för enkla operationer som aldrig behöver ångras, köas eller upprepas. Att wrappa `lamp.TurnOn()` i ett Command-objekt för ett skript som körs en gång är bara extra kod.
- **Singleton** bör undvikas om tillståndet påverkar testbarhet. En global instans gör enhetstester svårare eftersom sidoeffekter läcker mellan tester. Dependency Injection är ofta ett bättre val.
- **Strategy** är överengineering om det bara finns ett beteende. Tre klasser för en algoritm som aldrig byts ut är sämre än en enkel if-sats.
- **Facade** kan dölja för mycket. Om all kod bara pratar med Facade-lagret kan det bli svårt att förstå vad som faktiskt händer inuti systemet.

Principen: välj mönster när problemet *redan finns*, inte för att koden "kanske" behöver det i framtiden.

---

## Klassdiagram

Se `class-diagram.png` i repot.

```
IDevice ◄──── BaseDevice ◄──── Lamp
                │               Thermostat
                │               DoorLock
                │
         ISmartHomeSubject
                │
         notifies ──► ISmartHomeObserver
                          Dashboard
                          EventLogger  ──► SmartHomeLogger (Singleton)
                          AuditLog

ICommand ◄──── TurnOnCommand
               TurnOffCommand        ──► CommandInvoker (Queue + Stack)
               SetTemperatureCommand
               LockDoorCommand

IModeStrategy ◄── NormalMode
                  EcoMode
                  PartyMode

SmartHomeFacade ──► DeviceFactory (Factory Method)
                ──► CommandInvoker
                ──► IModeStrategy
                ──► ISmartHomeSubject / IDevice
```

---

## Demo output (urval)

```
--- 2. Observer – TurnOn triggers Dashboard + Logger + AuditLog ---
  [INVOKER]   ▶ Executing: TurnOn(LivingRoomLamp)
  [DASHBOARD] LivingRoomLamp turned ON
  [LOG]       [EVENT] Device=LivingRoomLamp, Type=TurnOn
  [AUDIT]     LivingRoomLamp turned ON

--- 4. Strategy – EcoMode blocks temperature above 21°C ---
  [ECO MODE]  ❌ Temperature 25°C exceeds eco limit (21°C).

  Undoing last command:
  [UNDO]      ↩ Undid TurnOn – BedroomLamp turned OFF

  Replaying last 3 commands from history:
  [REPLAY]    ▶ Re-executing: TurnOn(LivingRoomLamp)
  [REPLAY]    ▶ Re-executing: SetTemperature(MainThermostat, 23°C)
  [REPLAY]    ▶ Re-executing: LockDoor(FrontDoor)

--- 6. Singleton – Same Logger instance everywhere ---
  loggerA == loggerB: True
  Total log entries so far: 57
```
