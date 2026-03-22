using SmartHomeHub.Devices;

namespace SmartHomeHub.Commands;

public class LockDoorCommand : ICommand
{
    private readonly DoorLock _door;

    public string CommandName => $"LockDoor({_door.Name})";

    public LockDoorCommand(DoorLock door)
    {
        _door = door;
    }

    public void Execute()
    {
        _door.Lock();
    }

    public void Undo()
    {
        _door.Unlock();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  [UNDO]      ↩️  Undid Lock – {_door.Name} UNLOCKED");
        Console.ResetColor();
    }
}
