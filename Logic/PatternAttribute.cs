using System.Text.RegularExpressions;

namespace Logic;

public class PatternAttribute : Attribute
{
    static readonly Dictionary<string, Regex> Patterns = new();
    public readonly Regex Pattern;

    public PatternAttribute(string pattern)
    {
        if (!Patterns.TryGetValue(pattern, out var regex))
        {
            regex = new Regex($"^{pattern}$", RegexOptions.Compiled);
            Patterns.Add(pattern, regex);
        }
        Pattern = regex;
    }
}
