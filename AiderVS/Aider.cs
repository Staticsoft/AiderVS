namespace AiderVS;

public class Aider
{
    readonly string WorkingDirectory;
    readonly Logger Output;

    public Aider(string workingDirectory, Logger output)
    {
        WorkingDirectory = workingDirectory;
        Output = output;
    }
}