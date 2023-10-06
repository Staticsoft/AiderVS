using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Testing;
using System.Threading.Channels;

namespace Logic.Tests;

public class CliCommandExecutorTests : TestBase<CliCommandExecutor>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<CliCommandExecutor>()
        .AddSingleton<CliFake>()
        .ReuseSingleton<Cli, CliFake>();

    [Test]
    public async Task ReturnsOutputFromSingleCommand()
    {
        Get<CliFake>().RegisterResponse("greet", "Hello, World!");
        var response = await SUT.Execute("greet");
    }
}

public class CliCommandExecutor
{
    readonly Cli Cli;

    public CliCommandExecutor(Cli cli)
        => Cli = cli;

    public async Task<string> Execute(string command)
    {
        throw new NotImplementedException();
    }
}

public interface Cli
{
    ChannelReader<string> Messages { get; }
    Task Write(string message);
}

public class CliFake : Cli
{
    readonly Channel<string> Messages = Channel.CreateUnbounded<string>();
    readonly Dictionary<string, string[]> Responses = new();

    ChannelReader<string> Cli.Messages
        => Messages;

    public async Task Write(string message)
    {
        await Task.Delay(1000);
        foreach (var response in Responses[message])
        {
            await Messages.Writer.WriteAsync(response);
        }
    }

    public void RegisterResponse(string message, string response)
        => RegisterResponse(message, new[] { response });

    public void RegisterResponse(string message, string[] response)
        => Responses[message] = response;
}