
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.RegularExpressions.Templating;
using System.Text;

namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;

public class FormatWriter
    {
    private readonly IDictionary<string, string> dictionary;
    private readonly TextWriter textWriter;
    private readonly StringBuilder keyBuilder;

    public char OpenChar { get; }
    public char CloseChar { get; }

    public FormatWriter(object args, TextWriter textWriter, StringBuilder keyBuilder, char openChar = '{', char closeChar = '}', string nullString = "")
        : this(args.GetPropertiesDictionary(), textWriter, keyBuilder, openChar, closeChar, nullString) { }

    public FormatWriter(IDictionary<string, object?> dictionary, TextWriter textWriter, StringBuilder keyBuilder, char openChar = '{', char closeChar = '}', string nullString = "")
        : this(dictionary.ToDictionary(pair => pair.Key, pair => pair.Value?.ToInvariantString() ?? nullString), textWriter, keyBuilder, openChar, closeChar) { }

    public FormatWriter(IDictionary<string, string> dictionary, TextWriter textWriter, StringBuilder keyBuilder, char openChar = '{', char closeChar = '}')
        {
        if ((this.OpenChar = openChar) == (this.CloseChar = closeChar))
            {
            throw new InvalidOperationException("openChar and closeChar must be different.");
            }

        this.dictionary = dictionary;
        this.textWriter = textWriter;
        this.keyBuilder = keyBuilder;
        }

    public void ClearKey()
        => this.keyBuilder.Clear();

    public void WriteToKey(char c)
        => this.keyBuilder.Append(c);

    public void WriteKeyToOutput()
        {
        this.WriteOpenChar();
        this.textWriter.Write(this.keyBuilder.ToString());
        }

    public void WriteOpenChar()
        => this.Write(this.OpenChar);

    public void Write(char c)
        => this.textWriter.Write(c);

    public void WriteFromDictionary()
        {
        string key = this.keyBuilder.ToString();
        if (this.dictionary.TryGetValue(key, out string? value))
            {
            this.textWriter.Write(value);
            }
        else
            {
            this.textWriter.Write(this.OpenChar);
            this.textWriter.Write(key);
            this.textWriter.Write(this.CloseChar);
            }
        }
    }
