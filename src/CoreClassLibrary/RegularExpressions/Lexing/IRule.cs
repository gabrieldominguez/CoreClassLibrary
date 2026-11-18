namespace CoreClassLibrary.RegularExpressions.Lexing;

public interface IRule<T>
    {
    public RuleMatch<T>? Match(string input);
    }
