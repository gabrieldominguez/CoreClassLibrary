namespace CoreClassLibrary.RegularExpressions.Lexing.Rules;

public class StringRule<T>(String str, T token) : IRule<T>
    {
    public RuleMatch<T>? Match(string input) 
        => input.StartsWith(str) ?
            new RuleMatch<T>(str, token) :
            default;
    }
