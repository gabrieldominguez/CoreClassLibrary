using System.Text.RegularExpressions;

namespace CoreClassLibrary.RegularExpressions.Lexing.Rules;

public record RegexRule<T>(string Pattern, T Token) : IRule<T>
    {
    private readonly Regex regex = new Regex(string.Concat("^", Pattern));

    public RuleMatch<T>? Match(string input)
        {
        Match match = this.regex.Match(input);
        return match.Success ?
            new RuleMatch<T>(match.Value, this.Token) :
            default;
        }
    }
