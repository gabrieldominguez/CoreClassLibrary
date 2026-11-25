using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Alinea a la derecha los caracteres de la instancia e inserta a la izquierda espacios en blanco o un carácter especificado hasta alcanzar la longitud total especificada.
/// </summary>
public class PadLeftPipe : IPipe<string>
    {
    /// <summary>
    /// Número de caracteres de la cadena resultante, igual al número de caracteres originales más el número de caracteres de relleno adicionales. 
    /// </summary>
    public int TotalWidth { get; set; } = 0;

    /// <summary>
    /// Carácter de relleno.
    /// </summary>
    public char PaddingChar { get; set; } = ' ';

    #region Constructors
    public PadLeftPipe() { }

    public PadLeftPipe(int totalWidth) : this(totalWidth, ' ') { }

    public PadLeftPipe(int totalWidth, char paddingChar)
        {
        this.TotalWidth = totalWidth;
        this.PaddingChar = paddingChar;
        }
    #endregion

    public string Transform(string input) => input.AlignRightOrTruncate(this.TotalWidth, this.PaddingChar);
    }
