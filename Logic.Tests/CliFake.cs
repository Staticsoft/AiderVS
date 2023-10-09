using System.Threading.Channels;

namespace Logic.Tests;

public class CliFake : Cli, Input, Output, IDisposable
{
    readonly Channel<string> Commands = Channel.CreateUnbounded<string>();
    readonly Channel<string> Output = Channel.CreateUnbounded<string>();
    readonly Dictionary<string, string[]> Responses = new();
    readonly CancellationTokenSource Cancellation = new();

    public event Action<string>? Sent;
    public event Action<string>? Received;

    public CliFake()
        => Task.Run(ReadCommands);

    ChannelReader<string> Cli.Output
        => Output;

    public async Task Write(string command)
    {
        Sent?.Invoke(command);
        await Commands.Writer.WriteAsync(command);
    }

    public void RegisterOutput(string command, string output)
        => RegisterOutput(command, new[] { output });

    public void RegisterOutput(string message, string[] output)
        => Responses[message] = output;

    async Task ReadCommands()
    {
        await Initialize();

        while (!Cancellation.IsCancellationRequested)
        {
            try
            {
                var command = await Commands.Reader.ReadAsync(Cancellation.Token);
                await ExecuteCommand(command);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    async Task Initialize()
    {
        await Output.Writer.WriteAsync($"{nameof(CliFake)} is initializing...");
        await Output.Writer.WriteAsync($"{nameof(CliFake)} is ready");
        await Output.Writer.WriteAsync("### READY FOR INPUT ###");
    }

    async Task ExecuteCommand(string command)
    {
        await SimulateExecution();
        var response = Responses[command].Append("### READY FOR INPUT ###");
        foreach (var line in response)
        {
            Received?.Invoke(line);
            await Output.Writer.WriteAsync(line);
        }
    }

    static Task SimulateExecution()
        => Task.Delay(1000);

    public void Dispose()
        => Cancellation.Cancel();
}