namespace CoreClassLibrary.RegularExpressions.Lexing.Rules;

public record CharRule<T>(char C, T Token) : IRule<T>
    {
    public RuleMatch<T>? Match(string input) 
        => input.Length > 0 && input[0] == this.C ?
            new RuleMatch<T>(this.C.ToString(), this.Token) :
            default;
    }
