using System.Reflection;
using System.Text.RegularExpressions;

namespace Logic;

public abstract class PatternHandlersBase : PatternHandlers
{
    readonly Lazy<IReadOnlyDictionary<Regex, Func<GroupCollection, Task>>> Handlers;

    public PatternHandlersBase()
        => Handlers = new(() => PatternHandlers.ToDictionary(handler => GetPatternAttribute(handler).Pattern));

    protected abstract IEnumerable<Func<GroupCollection, Task>> PatternHandlers { get; }

    static PatternAttribute GetPatternAttribute(Func<GroupCollection, Task> handler)
        => handler.Method
            .GetParameters()
            .Single()
            .GetCustomAttribute<PatternAttribute>() ?? throw NoPatternAttributeError();

    public IEnumerable<Regex> Patterns
        => Handlers.Value.Keys;

    public Func<GroupCollection, Task> this[Regex pattern]
        => Handlers.Value[pattern];

    static MissingMemberException NoPatternAttributeError()
        => new($"{nameof(PatternAttribute)} is required for {nameof(GroupCollection)} parameter");
}