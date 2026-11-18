namespace CoreClassLibrary.RegularExpressions.Lexing;

public record LexerMatch<T>(RuleMatch<T> Match, int Position = 0, int LineNumber = 1);

public record RuleMatch<T>(string Value, T? Token)
    {
    public override string ToString() => Value;
    }

