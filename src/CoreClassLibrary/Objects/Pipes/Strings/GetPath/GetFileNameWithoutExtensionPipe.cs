namespace CoreClassLibrary.Objects.Pipes.Strings.GetPath;

/// <summary>
/// Retorna el nombre de archivo sin la extensión del path.
/// </summary>
public class GetFileNameWithoutExtensionPipe : IPipe<string>
    {
    public string Transform(string input) => Path.GetFileNameWithoutExtension(input);
    }
