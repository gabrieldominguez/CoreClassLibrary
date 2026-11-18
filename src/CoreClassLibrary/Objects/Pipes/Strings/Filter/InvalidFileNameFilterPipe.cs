namespace CoreClassLibrary.Objects.Pipes.Strings.Filter;

/// <summary>
/// Filtra la entrada que contiene carácteres inválidos para el nombre de una carpeta o archivo.
/// </summary>
public class InvalidFileNameFilterPipe : ContainsCharsFilterPipe
    {
    #region Constructors
    public InvalidFileNameFilterPipe() : base(Path.GetInvalidFileNameChars()) {}

    public InvalidFileNameFilterPipe(bool invert): base(Path.GetInvalidFileNameChars(), invert) {}
    #endregion 

    /// <summary>
    /// La propiedad Filter de la clase base no puede usarse.
    /// </summary>
    public new string Filter { get { return "";} }
    }
