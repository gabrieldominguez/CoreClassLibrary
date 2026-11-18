namespace CoreClassLibrary.Objects.Pipes.Strings.Filter;

/// <summary>
/// Filtra la entrada que contiene carácteres especificados.
/// </summary>
public class ContainsCharsFilterPipe : FilterPipe
    {
    #region Properties
    private char[] filterChars = [];

    /// <summary>
    /// Caracteres contenidos en la entrada a filtrar.
    /// </summary>
    public new string Filter
        {
        get { return this.filter; }
        set
            {
            this.filterChars = (this.filter = value).ToCharArray();
            }
        }
    #endregion

    #region Constructors
    #region string filter
    public ContainsCharsFilterPipe(string filter) : this(filter, false) {}

    public ContainsCharsFilterPipe(string filter, bool invert) : this(filter, invert, false) {}

    public ContainsCharsFilterPipe(string filter, bool invert, bool nullifyEmptyOutput): base(filter, invert, nullifyEmptyOutput)
        {
        this.Filter = filter;
        }
    #endregion

    #region char[] filter
    public ContainsCharsFilterPipe() {}

    public ContainsCharsFilterPipe(char[] filter) : this(filter, false) {}

    public ContainsCharsFilterPipe(char[] filter, bool invert): base(new string(filter), false, invert)
        {
        this.filterChars = filter;
        }
    #endregion
    #endregion

    protected override bool ApplyFilter(string input)
        => input.IndexOfAny(this.filterChars) != -1;
    }
