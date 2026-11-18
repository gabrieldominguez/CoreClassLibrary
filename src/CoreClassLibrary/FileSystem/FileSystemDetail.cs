namespace CoreClassLibrary.FileSystem;

/// <summary>
/// Usar en lugar de FileSystemInfo cuando se requiera serializar las propiedades DateTime.
/// </summary>
/// <remarks>
/// Las propiedades DateTime FileSystemInfo
/// </remarks>
public class FileSystemDetail
    {
    public string Name { get; set; } = "";
    public string FullName { get; set; } = "";

    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }

    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }

    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    }
