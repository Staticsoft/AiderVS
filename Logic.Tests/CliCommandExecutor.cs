using System.Threading.Channels;

namespace Logic.Tests;

public class CliCommandExecutor : IDisposable
{
    readonly Channel<ScheduledCommand> Commands = Channel.CreateUnbounded<ScheduledCommand>();
    readonly Channel<string> Executions = Channel.CreateUnbounded<string>();
    readonly CancellationTokenSource Cancellation = new();
    readonly Cli Cli;

    public CliCommandExecutor(Cli cli)
    {
        Cli = cli;
        Task.Run(AnalyzeOutput);
        Task.Run(ExecuteCommands);
    }

    public async Task<string> Execute(string command)
    {
        var taskSource = new TaskCompletionSource<string>();
        await Commands.Writer.WriteAsync(new(command, taskSource));
        return await taskSource.Task;
    }

    async Task AnalyzeOutput()
    {
        while (!Cancellation.IsCancellationRequested)
        {
            try
            {
                var output = await ReadUntilNextInput();
                await Executions.Writer.WriteAsync(output);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    async Task<string> ReadUntilNextInput()
    {
        var lines = new List<string>();
        while (await Cli.Output.WaitToReadAsync())
        {
            var line = await Cli.Output.ReadAsync();
            if (line == "### READY FOR INPUT ###") break;

            lines.Add(line);
        }
        return string.Join(Environment.NewLine, lines);
    }

    async Task ExecuteCommands()
    {
        await Initialize();

        while (!Cancellation.IsCancellationRequested)
        {
            try
            {
                var command = await Commands.Reader.ReadAsync();
                await Cli.Write(command.Text);
                var execution = await Executions.Reader.ReadAsync();
                command.Result.SetResult(execution);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    async Task Initialize()
    {
        await Executions.Reader.ReadAsync();
    }

    public void Dispose()
        => Cancellation.Cancel();
}
