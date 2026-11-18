using CoreClassLibrary.FileSystem;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.GetPath;

/// <summary>
/// Retorna el path y nombre de archivo sin la extensión.
/// </summary>
public class GetFileFullNameWithoutExtensionPipe : IPipe<string>
    {
    public string Transform(string input) => input.GetFullPathWithoutExtension();
    }
