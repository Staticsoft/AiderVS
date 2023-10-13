namespace Logic.Tests;

public class TestAiderOptions : AiderOptions
{
    public TestAiderOptions()
        : base(GetSolutionDirectory()) { }

    static string GetSolutionDirectory()
    {
        var directory = Directory.GetParent(Directory.GetCurrentDirectory());
        while (directory != null && !directory.EnumerateFiles().Any(file => file.Name.EndsWith(".sln")))
        {
            directory = directory.Parent;
        }
        if (directory == null) throw NoSolutionDirectoryFound();

        return directory.FullName;
    }

    static InvalidOperationException NoSolutionDirectoryFound()
        => new("Unable to locate solution directory");
}
