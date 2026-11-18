using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Get;

/// <summary>
/// Recupera una subcadena de la entrada.
/// </summary>
public class SubstringPipe : IPipe<string>
    {
    /// <summary>
    /// Posición en la cual obtener la cadena.
    /// </summary>
    /// <remarks>
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena
    /// </remarks>
    public int StartIndex { get; set; } = 0;

    /// <summary>
    /// Número de carácteres de la subcadena a obtener
    /// </summary>
    /// <remarks>
    /// 0: hasta el final de la cadena
    /// n: n carácteres delante de StartIndex
    /// -n: n carácteres detrás de StartIndex
    /// </remarks>
    public int Length { get; set; } = 0;

    #region Constructors
    public SubstringPipe() { }

    public SubstringPipe(int startIndex) : this(startIndex, 0) { }

    public SubstringPipe(int startIndex, int length)
        {
        this.StartIndex = startIndex;
        this.Length = length;
        }
    #endregion 

    public string Transform(string input)
        => input.Substring2(this.StartIndex, this.Length);
    }
