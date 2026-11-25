namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Aplica String.Format(), donde "{0}" representa a la entrada
/// </summary>
public class FormatPipe : IPipe<string>
    {
    /// <summary>
    /// Cadena de formato a aplicar a la entrada.
    /// </summary>
    public string Format { get; set; } = "{0}";

    #region Constructors
    public FormatPipe() { }

    public FormatPipe(string format)
        {
        this.Format = format;
        }
    #endregion 

    public string Transform(string input)
        => String.Format(this.Format, input);
    }
