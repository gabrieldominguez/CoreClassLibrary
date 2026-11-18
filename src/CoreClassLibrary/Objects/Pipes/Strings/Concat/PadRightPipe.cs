using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Alinea a la izquierda los caracteres de la cadena e inserta a la derecha un carácter especificado hasta alcanzar la longitud total especificada.
/// </summary>
public class PadRightPipe : IPipe<string>
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
    public PadRightPipe() { }

    public PadRightPipe(int totalWidth) : this(totalWidth, ' ') { }

    public PadRightPipe(int totalWidth, char paddingChar)
        {
        this.TotalWidth = totalWidth;
        this.PaddingChar = paddingChar;
        }
    #endregion

    public string Transform(string input) => input.AlignLeftOrTruncate(this.TotalWidth, this.PaddingChar);
    }
