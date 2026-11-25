
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.RegularExpressions.Templating;
using System.Text;

namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;

public class ParamWriter(IDictionary<string, string> dictionary, TextWriter textWriter, StringBuilder keyBuilder, char openChar = '@')
    {
    public char OpenChar { get; } = openChar;

    public ParamWriter(object args, TextWriter textWriter, StringBuilder keyBuilder, char openChar = '@', string nullString = "")
        : this(args.GetPropertiesDictionary(), textWriter, keyBuilder, openChar, nullString) { }

    public ParamWriter(IDictionary<string, object?> dictionary, TextWriter textWriter, StringBuilder keyBuilder, char openChar = '@', string nullString = "")
        : this(dictionary.ToDictionary(pair => pair.Key, pair => pair.Value?.ToInvariantString() ?? nullString), textWriter, keyBuilder, openChar) { }

    public void ClearKey()
        => keyBuilder.Clear();

    public void WriteToKey(char c)
        => keyBuilder.Append(c);

    public void WriteKeyToOutput()
        {
        this.WriteOpenChar();
        textWriter.Write(keyBuilder.ToString());
        }

    public void WriteOpenChar()
        => this.Write(this.OpenChar);

    public void Write(char c)
        => textWriter.Write(c);

    public void WriteFromDictionary()
        {
        string key = keyBuilder.ToString();
        if(dictionary.TryGetValue(key, out string? value))
            {
            textWriter.Write(value);
            }
        else
            {
            textWriter.Write(this.OpenChar);
            textWriter.Write(key);
            }
        }
    }
