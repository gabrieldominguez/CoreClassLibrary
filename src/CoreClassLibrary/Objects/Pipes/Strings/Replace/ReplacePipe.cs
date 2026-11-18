using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Reemplaza todas las apariciones de una cadena en la entrada por otra cadena.
/// </summary>
public class ReplacePipe : IPipe<string>
    {
    /// <summary>
    /// Cadena que se va a reemplazar
    /// </summary>
    public string OldValue { get; set; } = "";
    /// <summary>
    /// Cadena que va a reemplazar todas las apariciones de OldValue.
    /// </summary>
    public string NewValue { get; set; } = "";

    #region Constructors
    public ReplacePipe() { }

    public ReplacePipe(string oldValue) : this(oldValue, "") { }

    public ReplacePipe(string oldValue, string newValue)
        {
        this.OldValue = oldValue;
        this.NewValue = newValue;
        }
    #endregion 

    public string Transform(string input)
        => input.Length == 0 || this.OldValue.Length == 0 ? input : input.Replace(this.OldValue, this.NewValue);
    }
