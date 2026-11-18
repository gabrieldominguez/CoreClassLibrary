using CoreClassLibrary.FileSystem;

namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Reemplaza todos los carácteres inválidos para el nombre de una carpeta o archivo por un nuevo carácter.
/// </summary>
public class ReplaceInvalidFileNameCharsPipe : IPipe<string>
    {
    /// <summary>
    /// Carácter de reemplazo.
    /// </summary>
    public char NewChar { get; set; } = '_';

    #region Constructors
    public ReplaceInvalidFileNameCharsPipe() {}

    public ReplaceInvalidFileNameCharsPipe(char newChar)
        {
        this.NewChar = newChar;
        }
    #endregion

    public string Transform(string input) => input.ReplaceInvalidFileNameChars(this.NewChar);
    }
