using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Uris;

/// <summary>
/// Combina el PathAndQuery del uri del request con la uri especificada.
/// </summary>
/// <see cref="https://stackoverflow.com/questions/372865/path-combine-for-urls"/>
/// <see cref="https://stackoverflow.com/questions/7307833/creating-an-uri-in-net-automatically-urldecodes-all-parameters-from-passed-stri/7307950#7307950"/>
public class CombineUriPipe(Uri baseUri) : Pipe<Uri>(uri => new Uri(baseUri, uri.PathAndQuery))
    {
    }
