using CoreClassLibrary.RegularExpressions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Uris;

/// <summary>
/// Crea un nuevo Uri con el scheme, host y port especificado y un reemplazo en el path del parametro.
/// </summary>
public class ReplacePathUriPipe(string scheme, string host, int port, string search, string replacement) : Pipe<Uri>(uri => new UriBuilder(scheme, host, port, uri.PathAndQuery?.ReplaceIgnoringCase(search, replacement), uri.Fragment).Uri)
    {
    public ReplacePathUriPipe(Uri uri, string search, string replacement) : 
        this(uri.Scheme, uri.Host, uri.Port, search, replacement) { }
    }
