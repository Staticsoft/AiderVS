using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Testing;

namespace Logic.Tests;

public class CliCommandExecutorTests : TestBase<CliCommandExecutor>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<CliCommandExecutor>()
        .AddSingleton<CliFake>()
        .ReuseSingleton<Cli, CliFake>();

    void SetCommandOutput(string comand, string output)
        => Get<CliFake>().RegisterOutput(comand, output);

    void SetCommandOutput(string command, string[] output)
        => Get<CliFake>().RegisterOutput(command, output);

    [Test]
    public async Task ReturnsOutputFromSingleCommand()
    {
        SetCommandOutput("greet", "Hello, developer!");
        var output = await SUT.Execute("greet");
        Assert.Equal("Hello, developer!", output);
    }

    [Test]
    public async Task ReturnsOutputFromMultipleCommands()
    {
        SetCommandOutput("greet", "Hello, developer!");
        SetCommandOutput("feed", "Here is your food!");
        SetCommandOutput("sleep", "Good night!");
        var greetCommand = SUT.Execute("greet");
        var feedCommand = SUT.Execute("feed");
        var sleepCommand = SUT.Execute("sleep");
        Assert.Equal("Hello, developer!", await greetCommand);
        Assert.Equal("Here is your food!", await feedCommand);
        Assert.Equal("Good night!", await sleepCommand);
    }

    [Test]
    public async Task ReturnsOutputFromMultilineCommand()
    {
        var greetings = new[] { "Hello, developer", "Hello, user" };
        SetCommandOutput("greet", greetings);
        var output = await SUT.Execute("greet");
        Assert.Equal(output, string.Join(Environment.NewLine, greetings));
    }

    [Test]
    public async Task ReturnsOutputFromMultipleMultilineCommands()
    {
        var greetings = new[] { "Hello, developer", "Hello, user" };
        var help = new[] { "--help: Prints help information", "--action: Does an action" };
        SetCommandOutput("greet", greetings);
        SetCommandOutput("help", help);
        var greetingsCommand = SUT.Execute("greet");
        var helpCommand = SUT.Execute("help");
        Assert.Equal(string.Join(Environment.NewLine, greetings), await greetingsCommand);
        Assert.Equal(string.Join(Environment.NewLine, help), await helpCommand);
    }
}
