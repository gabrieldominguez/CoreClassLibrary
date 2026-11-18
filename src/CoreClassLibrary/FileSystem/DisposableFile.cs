namespace CoreClassLibrary.FileSystem;

public class DisposableFile : IDisposable
    {
    public string Path { get; }

    public DisposableFile(string nameFormat = "{0}", string contents = "")
        {
        this.Path = String.Format(nameFormat, Guid.NewGuid());
        File.WriteAllText(this.Path, contents);
        }

    public void Dispose()
        {
        File.Delete(this.Path);

        GC.SuppressFinalize(this);
        }
    }
