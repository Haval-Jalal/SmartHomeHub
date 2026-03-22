using SmartHomeHub.Devices;
using SmartHomeHub.Strategies;

namespace SmartHomeHub.Commands;

public class TurnOnCommand : ICommand
{
    private readonly IDevice _device;
    private readonly IModeStrategy _mode;

    public string CommandName => $"TurnOn({_device.Name})";

    public TurnOnCommand(IDevice device, IModeStrategy mode)
    {
        _device = device;
        _mode = mode;
    }

    public void Execute()
    {
        if (!_mode.CanTurnOnDevice(_device.Name))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [BLOCKED]   ❌ Mode '{_mode.ModeName}' blocked TurnOn for {_device.Name}");
            Console.ResetColor();
            return;
        }
        _device.TurnOn();
    }

    public void Undo()
    {
        _device.TurnOff();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  [UNDO]      ↩️  Undid TurnOn – {_device.Name} turned OFF");
        Console.ResetColor();
    }
}
