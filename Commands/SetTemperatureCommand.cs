using SmartHomeHub.Devices;
using SmartHomeHub.Strategies;

namespace SmartHomeHub.Commands;

public class SetTemperatureCommand : ICommand
{
    private readonly Thermostat _thermostat;
    private readonly double _targetTemp;
    private readonly double _previousTemp;
    private readonly IModeStrategy _mode;

    public string CommandName => $"SetTemperature({_thermostat.Name}, {_targetTemp}°C)";

    public SetTemperatureCommand(Thermostat thermostat, double targetTemp, IModeStrategy mode)
    {
        _thermostat = thermostat;
        _targetTemp = targetTemp;
        _previousTemp = thermostat.TargetTemperature;
        _mode = mode;
    }

    public void Execute()
    {
        if (!_mode.CanChangeTemperature(_thermostat.TargetTemperature, _targetTemp))
            return;

        _thermostat.SetTemperature(_targetTemp);
    }

    public void Undo()
    {
        _thermostat.SetTemperature(_previousTemp);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  [UNDO]      ↩️  Undid temperature change – {_thermostat.Name} back to {_previousTemp}°C");
        Console.ResetColor();
    }
}
