namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Inserta una cadena en la entrada.
/// </summary>
public class InsertPipe : IPipe<string>
    {
    public int StartIndex { get; set; } = 0;
    public string ToInsert { get; set; } = "";

    #region Constructors
    public InsertPipe() { }

    public InsertPipe(string toInsert) : this(toInsert, 0) { }

    public InsertPipe(string toInsert, int startIndex)
        {
        this.ToInsert = toInsert;
        this.StartIndex = startIndex;
        }
    #endregion  

    public string Transform(string input)
        => Extensions.StringExtensions.Insert2(input, this.ToInsert, this.StartIndex);
    }
