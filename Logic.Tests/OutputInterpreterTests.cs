using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Testing;
using System.Text.RegularExpressions;

namespace Logic.Tests;

public class OutputInterpreterTests : TestBase<OutputInterpreter<TestPatternHandlers>>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<TestPatternHandlers>()
        .AddSingleton<OutputInterpreter<TestPatternHandlers>>();

    TestPatternHandlers Handlers
        => Get<TestPatternHandlers>();

    [Test]
    public async Task DoesNothingWhenNoPatternHandlersProvided()
    {
        await SUT.Interpret("line that does not match");
        Assert.Null(Handlers.Received);
    }

    [Test]
    public async Task InvokesHandlerWhenMatchHappens()
    {
        await SUT.Interpret("123");
        Assert.NotNull(Handlers.Received);
        Assert.Equal("123", Handlers.Received!["digits"].Value);
    }
}

public class TestPatternHandlers : PatternHandlersBase
{
    public GroupCollection? Received { get; private set; }

    public TestPatternHandlers()
        => PatternHandlers = new[]
        {
            LettersHandler,
            DigitsHandler
        };

    protected override IEnumerable<Func<GroupCollection, Task>> PatternHandlers { get; }

    Task LettersHandler([Pattern("(?<letters>[a-zA-Z]+)")] GroupCollection matches)
    {
        Received = matches;
        return Task.CompletedTask;
    }

    Task DigitsHandler([Pattern("(?<digits>[0-9]+)")] GroupCollection matches)
    {
        Received = matches;
        return Task.CompletedTask;
    }
}