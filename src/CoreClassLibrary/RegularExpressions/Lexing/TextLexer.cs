namespace CoreClassLibrary.RegularExpressions.Lexing;

/// <summary>
/// Multiline Lexer
/// </summary>
/// <see cref="https://www.javacodegeeks.com/2017/09/guide-parsing-algorithms-terminology.html"/>
/// <see cref="https://tomassetti.me/parsing-in-csharp/"/>
/// <see cref="https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp"/>
/// <seealso cref="How to parse a file - Matt Ellis"/>
/// <seealso cref="https://www.youtube.com/watch?v=0b3PiOoHUp0"/>
public class TextLexer<T>(TextReader reader, Func<string, ILexer<T>> lexerFactory): ILexer<T>
    {
    private ILexer<T>? lineLexer = null;
    private int lineNumber = 0;

    private void TryCreateLineLexer()
        {
        string? line = reader.ReadLine();
        if (line != null)
            {
            this.lineLexer = lexerFactory.Invoke(line);
            this.lineNumber++;
            }
        else
            {
            this.lineLexer = null;
            }
        }

    public LexerMatch<T>? NextMatch()
        {
        if (this.lineLexer == null)
            {
            this.TryCreateLineLexer();
            }

        while (this.lineLexer != null)
            {
            try
                {
                LexerMatch<T>? lexerMatch = this.lineLexer.NextMatch();

                if (lexerMatch != null)
                    {
                    return lexerMatch with { LineNumber = this.lineNumber };
                    }
                else
                    {
                    this.TryCreateLineLexer();
                    }
                }
            catch (Exception e)
                {
                throw new InvalidOperationException($"Sintax error in line {this.lineNumber}.", e);
                }
            }

        return default;
        }
    }


