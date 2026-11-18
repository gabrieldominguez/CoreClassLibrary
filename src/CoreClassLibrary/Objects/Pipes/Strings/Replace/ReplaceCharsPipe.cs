using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pipes;


namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Reemplaza todos los carácteres especificados por un nuevo carácter.
/// </summary>
public class ReplaceCharsPipe : IPipe<string>
    {
    #region Properties
    private char[] oldChars = [];
    /// <summary>
    /// Caracteres a reemplazar.
    /// </summary>
    public string OldChars
        {
        get { return new String(this.oldChars); }
        set
            {
            this.oldChars = value.ToCharArray();
            }
        }

    /// <summary>
    /// Carácter de reemplazo. 
    /// </summary>
    public char NewChar { get; set; } = '_';
    #endregion

    #region Constructors

    #region string 
    public ReplaceCharsPipe() {}

    public ReplaceCharsPipe(string oldChars) : this(oldChars.ToCharArray()) {}

    public ReplaceCharsPipe(string oldChars, char newChar) : this(oldChars.ToCharArray(), newChar) {}
    #endregion

    #region char[] 
    public ReplaceCharsPipe(char[] oldChars) : this(oldChars, '_') {}

    public ReplaceCharsPipe(char[] oldChars, char newChar)
        {
        this.oldChars = oldChars;
        this.NewChar = newChar;
        }
    #endregion
    #endregion

    public string Transform(string input) => input.ReplaceChars(this.oldChars, this.NewChar);
    }
