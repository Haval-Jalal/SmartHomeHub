using SmartHomeHub.Devices;

namespace SmartHomeHub.Commands;

public class TurnOffCommand : ICommand
{
    private readonly IDevice _device;

    public string CommandName => $"TurnOff({_device.Name})";

    public TurnOffCommand(IDevice device)
    {
        _device = device;
    }

    public void Execute()
    {
        _device.TurnOff();
    }

    public void Undo()
    {
        _device.TurnOn();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  [UNDO]      ↩️  Undid TurnOff – {_device.Name} turned ON");
        Console.ResetColor();
    }
}
