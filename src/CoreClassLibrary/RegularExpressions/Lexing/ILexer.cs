namespace CoreClassLibrary.RegularExpressions.Lexing;

/// <summary>
/// Lexer
/// </summary>
/// <see cref="https://ruslanspivak.com/lsbasi-part1/"/>
/// <see cref="https://www.javacodegeeks.com/2017/09/guide-parsing-algorithms-terminology.html"/>
/// <see cref="https://tomassetti.me/parsing-in-csharp/"/>
/// <see cref="https://dev.to/j0nimost/series/14710"/>
/// <see cref="https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp"/>
/// <see cref="https://github.com/datalust/superpower"/>
/// <seealso cref="How to parse a file - Matt Ellis"/>
/// <seealso cref="https://www.youtube.com/watch?v=0b3PiOoHUp0"/>
public interface ILexer<T>
    {
    LexerMatch<T>? NextMatch();
    }

public static class LexerExtensions
    {    
    public static IEnumerable<LexerMatch<T>> GetMatches<T>(this ILexer<T> lexer)
        {
        LexerMatch<T>? lexerMatch;
        do
            {
            lexerMatch = lexer.NextMatch();

            if(lexerMatch != null)
                {
                yield return lexerMatch;
                }
            }
        while (lexerMatch != null);
        }
    }
