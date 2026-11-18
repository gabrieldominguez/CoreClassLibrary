namespace CoreClassLibrary.Objects.Pipes.Uris;

/// <summary>
/// Combina el uri del request con una uri especificada.
/// </summary>
/// <see cref="https://stackoverflow.com/questions/372865/path-combine-for-urls"/>
/// <see cref="https://stackoverflow.com/questions/7307833/creating-an-uri-in-net-automatically-urldecodes-all-parameters-from-passed-stri/7307950#7307950"/>
public class GetRelativeUriPipe : Pipe<Uri>
    {
    public GetRelativeUriPipe() :
        base(uri => new Uri(uri.PathAndQuery, UriKind.Relative)) { }
    }
