namespace CoreClassLibrary.Objects.Pipes.Strings.Filter;

/// <summary>
/// Filtra la entrada que comienza con el filtro.
/// </summary>
public class StartsWithFilterPipe : FilterPipe 
    {
    #region Constructors
    public StartsWithFilterPipe() { }

    public StartsWithFilterPipe(string filter) : base(filter) {}

    public StartsWithFilterPipe(string filter, bool ignoreCase) : base(filter, ignoreCase) {}

    public StartsWithFilterPipe(string filter, bool ignoreCase, bool invert): base(filter, ignoreCase, invert) {}
    #endregion

    protected override bool ApplyFilter(string input)
        => input.StartsWith(this.filter, this.caseComparison);
    }
