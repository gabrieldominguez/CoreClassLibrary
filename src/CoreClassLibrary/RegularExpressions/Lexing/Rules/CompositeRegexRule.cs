using System.Text.RegularExpressions;

namespace CoreClassLibrary.RegularExpressions.Lexing.Rules;

/// <summary>
/// Compone múltiples reglas de expresiones regulares en una única regla.
/// </summary>
/// <remarks>
/// Las expresiones regulares de las reglas a componer no pueden contener grupos '()'.
/// </remarks>
public class CompositeRegexRule<T>(string pattern, IReadOnlyList<T> tokens): IRule<T>
    {
    private readonly Regex regex = new Regex(pattern);

    public CompositeRegexRule(IEnumerable<RegexRule<T>> rules): this(
        rules.Select(rule => rule.Pattern).CombinePatterns(), 
        rules.Select(rule => rule.Token).ToArray()) { }

    public RuleMatch<T>? Match(string input)
        {
        Match match = this.regex.Match(input);
        return match.Success ?
            new RuleMatch<T>(match.Value, tokens[match.GetSuccessfulGroupIndex() - 1]):
            default;
        }
    }
