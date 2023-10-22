using System.Text.RegularExpressions;

namespace Logic;

public class OutputInterpreter<PatternHandlersCollection>
    where PatternHandlersCollection : PatternHandlers
{
    readonly PatternHandlersCollection Handlers;

    public OutputInterpreter(PatternHandlersCollection patterns)
        => Handlers = patterns;

    public async Task Interpret(string line)
    {
        foreach (var pattern in Handlers.Patterns)
        {
            var match = pattern.Match(line);
            if (match.Success)
            {
                await Handler(pattern)(match.Groups);
                break;
            }
        }
    }

    Func<GroupCollection, Task> Handler(Regex pattern)
        => Handlers[pattern];
}