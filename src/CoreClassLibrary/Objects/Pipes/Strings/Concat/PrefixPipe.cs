namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Aplica String.Concat()
/// </summary>
public class PrefixPipe : IPipe<string>
    {
    public string Prefix { get; set; } = "";

    #region Constructors
    public PrefixPipe() { }

    public PrefixPipe(string prefix)
        {
        this.Prefix = prefix;
        }
    #endregion 

    public string Transform(string input)
        => String.Concat(this.Prefix, input);
    }
