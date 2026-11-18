using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Get;

/// <summary>
/// Recupera una subcadena de la entrada.
/// </summary>
public class GetBetweenIndexesPipe : IPipe<string>
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
    /// Posición en la cual obtener la cadena.
    /// </summary>
    /// <remarks>
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena (-1: final de la cadena)
    /// </remarks>
    public int EndIndex { get; set; } = -1;

    #region Constructors
    public GetBetweenIndexesPipe() { }

    public GetBetweenIndexesPipe(int startIndex) : this(startIndex, -1) { }

    public GetBetweenIndexesPipe(int startIndex, int endIndex)
        {
        this.StartIndex = startIndex;
        this.EndIndex = endIndex;
        }
    #endregion 

    public string Transform(string input) => input.SubstringBetweenIndexes(this.StartIndex, this.EndIndex);
    }
