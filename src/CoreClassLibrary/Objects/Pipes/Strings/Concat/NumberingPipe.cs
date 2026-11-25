using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Numera las lineas de la entrada.
/// </summary>
public class NumberingPipe : IPipe<string>
    {
    /// <summary>
    /// Número de inicio del contador.
    /// </summary>
    public int Start { get; set; } = 1;

    /// <summary>
    /// Incremento del contador.
    /// </summary>
    /// <remarks>Si es negativo, la secuencia será decreciente.</remarks>
    public int Increment { get; set; } = 1;

    /// <summary>
    /// Indice de cada linea de la entrada en el cual insertar el numero correspondiente, formateado como cadena.
    /// </summary>
    public int StartIndex { get; set; } = 0;

    /// <summary>
    /// Formato a aplicar a cada numero de la secuencia, para convertirlo en texto.
    /// </summary>
    public string Format { get; set; } = "00 - ";

    #region Constructors
    public NumberingPipe() { }

    public NumberingPipe(string format): this(format, 0) {}

    public NumberingPipe(string format, int startIndex): this(format, startIndex, 1) {}

    public NumberingPipe(string format, int startIndex, int increment): this(format, startIndex, increment, 1) {}

    public NumberingPipe(string format, int startIndex, int increment, int start)
        {
        this.Format = format;
        this.StartIndex = startIndex;
        this.Increment = increment;
        this.Start = start;
        }
    #endregion 

    public string Transform(string input)
        {
        string[] linesInput = input.Split('\n');
        string[] linesOutput = new string[linesInput.Length];

        int number = this.Start;
        for (int i = 0; i  < linesInput.Length; i++)
            {                
            linesOutput[i] = linesInput[i].Insert2(number.ToString(this.Format), this.StartIndex);
            number += this.Increment;
            }

        return String.Join("\n", linesOutput);
        }
    }
