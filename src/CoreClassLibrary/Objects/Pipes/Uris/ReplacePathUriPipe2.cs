using CoreClassLibrary.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Uris;

/// <summary>
/// Crea un nuevo Uri haciendo un reemplazo en el path del parametro.
/// </summary>
public class ReplacePathUriPipe2(string search, string replacement) : Pipe<Uri>(uri => new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.PathAndQuery?.ReplaceIgnoringCase(search, replacement), uri.Fragment).Uri);
