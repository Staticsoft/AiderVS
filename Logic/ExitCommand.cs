namespace Logic;

public class ExitCommand
{
    readonly CliCommandExecutor Executor;

    public ExitCommand(CliCommandExecutor executor)
        => Executor = executor;

    public Task<string> Execute()
        => Executor.Execute("/exit");
}