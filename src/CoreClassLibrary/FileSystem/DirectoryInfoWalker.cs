using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.FileSystem;

/// <summary>
/// Visitor Pattern, para recorrer un arbol de directorios.
/// </summary>
public class DirectoryInfoWalker(string searchPattern)
    {
    public string SearchPattern { get; set; } = searchPattern;

    public DirectoryInfoWalker() : this("*") { }

    public bool Walk(Predicate<DirectoryInfo> visitor)
        => this.Walk(new DirectoryInfo(Directory.GetCurrentDirectory()), visitor);

    public bool Walk(DirectoryInfo directoryInfo, Predicate<DirectoryInfo> visitor)
        => directoryInfo.Walk(visitor, this.GetDirectories);

    private DirectoryInfo[] GetDirectories(DirectoryInfo directoryInfo)
        => directoryInfo.GetDirectories(this.SearchPattern);
    }
