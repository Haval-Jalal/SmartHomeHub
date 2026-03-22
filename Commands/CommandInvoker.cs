using SmartHomeHub.Singleton;

namespace SmartHomeHub.Commands;

/// <summary>
/// CommandInvoker – ansvarar för att köa, exekvera, logga och replay:a kommandon.
/// Håller historik för undo-funktionalitet (VG-krav).
/// </summary>
public class CommandInvoker
{
    private readonly Queue<ICommand> _commandQueue = new();
    private readonly Stack<ICommand> _history = new();
    private readonly SmartHomeLogger _logger = SmartHomeLogger.Instance;

    public void Enqueue(ICommand command)
    {
        _commandQueue.Enqueue(command);
        _logger.Log($"Queued command: {command.CommandName}");
    }

    public void ExecuteNext()
    {
        if (_commandQueue.Count == 0)
        {
            Console.WriteLine("  [INVOKER]   Queue is empty.");
            return;
        }

        var command = _commandQueue.Dequeue();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  [INVOKER]   ▶️  Executing: {command.CommandName}");
        Console.ResetColor();

        command.Execute();
        _history.Push(command);
        _logger.Log($"Executed command: {command.CommandName}");
    }

    public void ExecuteAll()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  [INVOKER]   ▶️  Running all {_commandQueue.Count} queued commands...");
        Console.ResetColor();

        while (_commandQueue.Count > 0)
            ExecuteNext();
    }

    /// <summary>
    /// Replay – kör om de senaste N kommandon från historiken (VG-krav).
    /// </summary>
    public void Replay(int count = 5)
    {
        var replayList = _history.Take(Math.Min(count, _history.Count)).Reverse().ToList();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"\n  [INVOKER]   🔁 Replaying last {replayList.Count} command(s)...");
        Console.ResetColor();

        foreach (var cmd in replayList)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  [REPLAY]    ▶️  Re-executing: {cmd.CommandName}");
            Console.ResetColor();
            cmd.Execute();
            _logger.Log($"Replayed command: {cmd.CommandName}");
        }
    }

    /// <summary>
    /// Undo – ångrar det senaste kommandot (VG-krav "undo light").
    /// </summary>
    public void Undo()
    {
        if (_history.Count == 0)
        {
            Console.WriteLine("  [INVOKER]   Nothing to undo.");
            return;
        }

        var command = _history.Pop();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  [INVOKER]   ↩️  Undoing: {command.CommandName}");
        Console.ResetColor();

        command.Undo();
        _logger.Log($"Undid command: {command.CommandName}");
    }

    public void PrintHistory()
    {
        Console.WriteLine("\n  === Command History ===");
        if (_history.Count == 0)
        {
            Console.WriteLine("  (empty)");
            return;
        }
        int i = 1;
        foreach (var cmd in _history)
            Console.WriteLine($"  {i++}. {cmd.CommandName}");
        Console.WriteLine("  ======================");
    }

    public int QueueCount => _commandQueue.Count;
    public int HistoryCount => _history.Count;
}
