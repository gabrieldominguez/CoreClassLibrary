namespace CoreClassLibrary.Objects.Pipes.Strings.Filter;

/// <summary>
/// Filtra la entrada que termina con el filtro.
/// </summary>
public class EndsWithFilterPipe : FilterPipe 
    {
    #region Constructors
    public EndsWithFilterPipe() { }

    public EndsWithFilterPipe(string filter) : base(filter) {}

    public EndsWithFilterPipe(string filter, bool ignoreCase) : base(filter, ignoreCase) {}

    public EndsWithFilterPipe(string filter, bool ignoreCase, bool invert): base(filter, ignoreCase, invert) {}
    #endregion

    protected override bool ApplyFilter(string input)
        => input.EndsWith(this.filter, this.caseComparison);
    }
