namespace CoreClassLibrary.Objects.Pipes.Strings.GetPath;

/// <summary>
/// Retorna el nombre de archivo y la extensión del path.
/// </summary>
public class GetFileNamePipe : IPipe<string>
    {
    public string Transform(string input) => Path.GetFileName(input);
    }
