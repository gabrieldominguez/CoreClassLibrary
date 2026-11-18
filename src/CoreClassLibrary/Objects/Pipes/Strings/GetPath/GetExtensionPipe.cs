namespace CoreClassLibrary.Objects.Pipes.Strings.GetPath;

/// <summary>
/// Retorna la extensión del path.
/// </summary>
public class GetExtensionPipe : IPipe<string>
    {
    public string Transform(string input) => Path.GetExtension(input);
    }
