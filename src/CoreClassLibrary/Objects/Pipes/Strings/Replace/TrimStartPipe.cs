using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Quita todas las ocurrencias de un caracter del final de la entrada.
/// </summary>
public class TrimStartPipe(char trimChar = ' ') : IPipe<string>
    {
    /// <summary>
    /// Caracter a quitar.
    /// </summary>
    public char TrimChar { get; set; } = trimChar;

    #region Constructors
    #endregion

    public string Transform(string input) => input.TrimStart(this.TrimChar);
    }
