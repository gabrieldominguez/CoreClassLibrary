namespace CoreClassLibrary.Objects.Pipes.Strings.GetPath;

/// <summary>
/// Retorna el directorio raiz del path.
/// </summary>
public class GetPathRootPipe : IPipe<string>
    {
    public string Transform(string input) => Path.GetPathRoot(input) ?? "";
    }
