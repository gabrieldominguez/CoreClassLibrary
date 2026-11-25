namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;

/// <summary>
/// Reemplaza nombres de propiedades comenzando con un token, por ej: @.
/// </summary>
/// <example>
/// "@CurrentTime - @ProcessName" => "1975-11-12 00:00:00 - ABC"
/// </example>
/// <see cref="https://stackoverflow.com/questions/159017/named-string-formatting-in-c-sharp"/>
/// <see cref="http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx"/>
/// <see cref="http://haacked.com/archive/2009/01/14/named-formats-redux.aspx/"/>
public class ParamSwitchStateMachine(ParamWriter writer)
    {
    public int State { get; private set; }

    public void Initialize()
        {
        this.State = States.InText;

        writer.ClearKey();
        }

    /// <summary>
    /// Reemplaza nombres de propiedades entre tokens distintos, por ej: llaves o corchetes.
    /// </summary>
    /// <example>
    /// "{CurrentTime} - {ProcessName}" => "1975-11-12 00:00:00 - ABC"
    /// </example>
    public bool Process(char c)
        {
        switch (this.State)
            {
            case States.InText:
                if (c == writer.OpenChar)
                    {
                    this.State = States.StartKey;
                    }
                else if (c == '\0')
                    {
                    }
                else
                    {
                    writer.Write(c);
                    }
                return false;

            case States.StartKey:
                if (Char.IsLetterOrDigit(c))
                    {
                    writer.ClearKey();
                    writer.WriteToKey(c);
                    this.State = States.InKey;
                    }
                else if (c == writer.OpenChar || c == '\0')
                    {
                    writer.WriteOpenChar();
                    }
                else
                    {
                    writer.WriteOpenChar();
                    writer.Write(c);
                    this.State = States.InText;
                    }
                return true;

            case States.InKey:
                if (Char.IsLetterOrDigit(c))
                    {
                    writer.WriteToKey(c);
                    }
                else if (c == writer.OpenChar || c == '\0')
                    {
                    writer.WriteFromDictionary();
                    this.State = States.StartKey;
                    }
                else
                    {
                    writer.WriteFromDictionary();
                    writer.Write(c);
                    this.State = States.InText;
                    }
                return true;
            }

        return false;
        }
    }
