namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Aplica String.Format(), donde "{0}" representa a la entrada y {1} el contenido del archivo FileNameArgument1 o en su defecto, la propiedad Argument1
/// </summary>
public class LineFormatPipe : IPipe<string>
    {
    /// <summary>
    /// Cadena de formato a aplicar a la entrada.
    /// </summary>
    public string Format { get; set; } = "{0}{1}";

    /// <summary>
    /// Archivo cuyo contenido se usará como argumento nro. 1
    /// </summary>
    public string FileNameArgument1 { get; set; } = "";
    /// <summary>
    /// Cadena a usar como argumento nro. 1. Se usa si no se especifica FileNameArgument1
    /// </summary>
    public string Argument1 { get; set; } = "";

    #region Constructors
    public LineFormatPipe() { }

    public LineFormatPipe(string format)
        {
        this.Format = format;
        }
    #endregion 

    public string Transform(string input)
        {
        string[] arg0 = input.Split(['\n']);
        string[] arg1 = (this.FileNameArgument1.Length == 0 ? this.Argument1: File.OpenText(this.FileNameArgument1).ReadToEnd()).Split(['\n']);

        string[] result = new string[Math.Min(arg0.Length, arg1.Length)];
		    for(int i = 0; i < result.Length; i++)
			    {
            result[i] = String.Format(this.Format, arg0[i], arg1[i]);
			    }

        return String.Join("\n", result);
        }
    }
