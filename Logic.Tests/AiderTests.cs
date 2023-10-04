using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Testing;

namespace Logic.Tests;

public class AiderTests : TestBase<Aider>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<Aider>()
        .AddSingleton<LoggerFake>()
        .ReuseSingleton<Logger, LoggerFake>()
        .AddSingleton(new AiderOptions(Directory.GetCurrentDirectory()));

    [Test]
    public void CanStartAndStopAider()
    {
        SUT.Dispose();
        Assert.NotEmpty(Get<LoggerFake>().Messages);
    }
}

public class LoggerFake : Logger
{
    readonly List<string> Logs = new();

    public void Log(string message)
        => Logs.Add(message);

    public IEnumerable<string> Messages
        => Logs;
}