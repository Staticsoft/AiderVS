using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Testing;
using Xunit;

namespace Logic.Tests;

public class ExitCommandTests : TestBase<ExitCommand>
{
    protected override IServiceCollection Services => base.Services
        .AddSingleton<ExitCommand>()
        .AddSingleton<CliCommandExecutor>()
        .AddSingleton<Cli, AiderCli>()
        .AddSingleton<AiderOptions, TestAiderOptions>();

    [Fact]
    public Task CanExitAider()
        => SUT.Execute();
}
