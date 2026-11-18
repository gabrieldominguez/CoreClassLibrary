namespace CoreClassLibrary.Objects.Pipes.Uris;

public class UriStringPipe(IPipe<string, string> stringPipe) : IPipe<Uri>
    {
    public Uri Transform(Uri x)
        => new Uri(stringPipe.Transform(x.ToString()));
    }
