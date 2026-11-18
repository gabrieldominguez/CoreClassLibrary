namespace CoreClassLibrary.RegularExpressions.Lexing.Rules;

public class CompositeRule<T>(IReadOnlyCollection<IRule<T>> rules): IRule<T>
    {
    public RuleMatch<T>? Match(string input)
        {
        foreach (IRule<T> rule in rules)
            {
            RuleMatch<T>? match = rule.Match(input);
            if (match != null)
                {
                return match;
                }
            }

        return default;
        }
    }
