namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Aplica String.Concat()
/// </summary>
public class SufixPipe : IPipe<string>
    {
    public string Sufix { get; set; } = "";

    #region Constructors
    public SufixPipe() { }

    public SufixPipe(string sufix)
        {
        this.Sufix = sufix;
        }
    #endregion 

    public string Transform(string input)
        => String.Concat(input, this.Sufix);
    }
