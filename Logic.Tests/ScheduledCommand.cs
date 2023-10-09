namespace Logic.Tests;

public class ScheduledCommand
{
    public readonly string Text;
    public readonly TaskCompletionSource<string> Result;

    public ScheduledCommand(string command, TaskCompletionSource<string> result)
    {
        Text = command;
        Result = result;
    }
}