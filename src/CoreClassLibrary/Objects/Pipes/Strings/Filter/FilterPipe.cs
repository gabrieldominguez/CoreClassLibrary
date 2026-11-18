using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Filter;

/// <summary>
/// Filtra la entrada que contiene al filtro (devolviendo "" o null).
/// </summary>
public class FilterPipe : IPipe<string>
    {
    #region Properties
    protected string filter = "";
    /// <summary>
    /// Texto a usar como filtro.
    /// </summary>
    public string Filter
        {
        get { return this.filter; }
        set { this.filter = value; }
        }

    protected StringComparison caseComparison = StringComparison.CurrentCulture;
    /// <summary>
    /// Ignora mayusculas y minusculas.
    /// </summary>
    public bool IgnoreCase
        {
        get { return this.caseComparison == StringComparison.CurrentCultureIgnoreCase; }
        set { this.caseComparison = value? StringComparison.CurrentCultureIgnoreCase: StringComparison.CurrentCulture; }
        }
    /// <summary>
    /// Invierte el filtro.
    /// </summary>
    public bool Invert { get; set; } = false;
    #endregion

    #region Constructors
    public FilterPipe() { }

    public FilterPipe(string filter) : this(filter, false){}

    public FilterPipe(string filter, bool ignoreCase) : this(filter, ignoreCase, false){}

    public FilterPipe(string filter, bool ignoreCase, bool invert)
        {
        this.Filter = filter;
        this.Invert = invert;
        this.IgnoreCase = ignoreCase;
        }
    #endregion 

    public string Transform(string input)
        => this.filter == "" || input.Length == 0 || this.Invert != this.ApplyFilter(input)? input: "";

    /// <summary>
    /// Aplica el filtro.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>True si la entrada debe pasar el filtro.</returns>
    protected virtual bool ApplyFilter(string input)
        => input.IndexOf(this.filter, this.caseComparison) != -1;
    }
