using System.Text.RegularExpressions;

namespace Logic;

public interface PatternHandlers
{
    IEnumerable<Regex> Patterns { get; }
    Func<GroupCollection, Task> this[Regex pattern] { get; }
}
