namespace Logic;

public class AiderCli : ProcessCli
{
    public AiderCli(AiderOptions options)
        : base(ToProcessOptions(options)) { }

    static ProcessCliOptions ToProcessOptions(AiderOptions options)
        => new()
        {
            Command = "python",
            Arguments = "-m aider.main --no-git --no-pretty --programmatic-access",
            WorkingDirectory = options.WorkingDirectory
        };
}

public class Aider
{
    public readonly ExitCommand Exit;

    public Aider(ExitCommand exit)
        => Exit = exit;
}