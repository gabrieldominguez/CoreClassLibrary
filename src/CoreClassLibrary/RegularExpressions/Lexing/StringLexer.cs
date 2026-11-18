namespace CoreClassLibrary.RegularExpressions.Lexing;

/// <summary>
/// Single line Lexer
/// </summary>
public class StringLexer<T>(string input, IRule<T> rule): ILexer<T>
    {
    private int position = 0;

    public LexerMatch<T>? NextMatch()
        {
        RuleMatch<T>? ruleMatch = rule.Match(input[this.position..]);

        if (ruleMatch != null) 
            {
            LexerMatch<T>? lexerMatch = new LexerMatch<T>(ruleMatch, this.position);
            this.position += ruleMatch.Value.Length;
            return lexerMatch;
            }

        return this.position < input.Length ? 
            throw new InvalidOperationException($"Sintax error in position {this.position}.") : 
            default;
        }
    }
