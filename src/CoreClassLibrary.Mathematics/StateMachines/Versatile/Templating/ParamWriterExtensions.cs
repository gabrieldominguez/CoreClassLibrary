namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;

/// <summary>
/// State machine implementations!.
/// </summary>
/// <see cref="https://stackoverflow.com/questions/159017/named-string-formatting-in-c-sharp"/>
/// <see cref="http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx"/>
/// <see cref="http://haacked.com/archive/2009/01/14/named-formats-redux.aspx/"/>
public static class ParamWriterExtensions
    {
    /// <summary>
    /// Reemplaza nombres de propiedades comenzando con un token, por ej: @.
    /// </summary>
    /// <example>
    /// "@CurrentTime - @ProcessName" => "1975-11-12 00:00:00 - ABC"
    /// </example>
    public static void Process(this ParamWriter writer, string input)
        {
        int state = States.InText;

        foreach (char c in input)
            {
            switch (state)
                {
                case States.InText:
                    if (c == writer.OpenChar)
                        {
                        state = States.StartKey;
                        }
                    else
                        {
                        writer.Write(c);
                        }
                    break;

                case States.StartKey:
                    if (Char.IsLetterOrDigit(c))
                        {
                        writer.ClearKey();
                        writer.WriteToKey(c);
                        state = States.InKey;
                        }
                    else if (c == writer.OpenChar)
                        {
                        writer.WriteOpenChar();
                        }
                    else
                        {
                        writer.WriteOpenChar();
                        writer.Write(c);
                        state = States.InText;
                        }
                    break;
                case States.InKey:
                    if (Char.IsLetterOrDigit(c))
                        {
                        writer.WriteToKey(c);
                        }
                    else if (c == writer.OpenChar)
                        {
                        writer.WriteFromDictionary();
                        state = States.StartKey;
                        }
                    else
                        {
                        writer.WriteFromDictionary();
                        writer.Write(c);
                        state = States.InText;
                        }
                    break;
                }
            }

        switch (state)
            {
            case States.StartKey:
                writer.WriteOpenChar();
                break;
            case States.InKey:
                writer.WriteFromDictionary();
                break;
            }
        }
    }
