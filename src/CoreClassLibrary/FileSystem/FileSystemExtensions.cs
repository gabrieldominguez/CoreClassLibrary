using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.FileSystem;

public enum EntryOption
    {
    All,
    Files,
    Directories
    }

public static class FileSystemExtensions
    {
    public static async Task<string> GetOrWriteAsync(string fileName, Func<CancellationToken, Task<string>> factoryAsync, CancellationToken cancellationToken = default)
        { 
        if (File.Exists(fileName))
            {
            return File.ReadAllText(fileName);
            }

        string text = await factoryAsync(cancellationToken);
        await File.WriteAllTextAsync(fileName, text, cancellationToken);
        return text;
        }

    public static string GetOrWrite(string fileName, Func<string> factory)
        { 
        if (File.Exists(fileName))
            {
            return File.ReadAllText(fileName);
            }

        string text = factory();
        File.WriteAllText(fileName, text);
        return text;
        }
    
    public static FileSystemDetail CreateDetail(FileSystemInfo fileSystemInfo)
        => new FileSystemDetail()
            {
            Name = fileSystemInfo.Name,
            FullName = fileSystemInfo.FullName,

            CreationTime = fileSystemInfo.CreationTime,
            CreationTimeUtc = fileSystemInfo.CreationTimeUtc,

            LastAccessTime = fileSystemInfo.LastAccessTime,
            LastAccessTimeUtc = fileSystemInfo.LastAccessTimeUtc,

            LastWriteTime = fileSystemInfo.LastWriteTime,
            LastWriteTimeUtc = fileSystemInfo.LastWriteTimeUtc,
            };

    public static FileSystemWatcher CreateFileChangeWatcher(string path, FileSystemEventHandler handler, bool enableRaisingEvents = true)
        {
        if (!Path.IsPathRooted(path))
            {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }

        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(path)!)
            {
            Filter = Path.GetFileName(path)
            };

        fileSystemWatcher.Changed += handler;
        fileSystemWatcher.EnableRaisingEvents = enableRaisingEvents;

        return fileSystemWatcher;
        }

    public static void Rename(this DirectoryInfo directory, string oldSubstring, string newSubstring)
        => directory.Rename(directory.Name.Replace(oldSubstring, newSubstring));

    public static void Rename(this DirectoryInfo directory, string newName)
        => directory.MoveTo(Path.Combine(directory.Parent?.FullName ?? "", newName));

    public static void RenameFiles(this DirectoryInfo directory, string oldSubstring, string newSubstring)
        => directory.EnumerateFiles($"*{oldSubstring}*").ForEach(file => file.Rename(oldSubstring, newSubstring));

    public static void Rename(this FileInfo file, string oldSubstring, string newSubstring)
        => file.Rename(file.Name.Replace(oldSubstring, newSubstring));

    public static void Rename(this FileInfo file, string newName)
        => file.MoveTo(Path.Combine(file.DirectoryName ?? "", newName));

    /// <summary>
    /// Returns an enumerable collection of file and/or directory info that match a search pattern in a specified path,
    /// and optionally searches subdirectories.
    /// </summary>
    public static IEnumerable<FileSystemDetail> EnumerateEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, EntryOption entryOption = EntryOption.All)
        => entryOption switch
            {
            EntryOption.All => new DirectoryInfo(path).EnumerateFileSystemInfos(searchPattern, searchOption).Select(FileSystemExtensions.CreateDetail),
            EntryOption.Files => new DirectoryInfo(path).EnumerateFiles(searchPattern, searchOption).Select(FileSystemExtensions.CreateDetail),
            EntryOption.Directories => new DirectoryInfo(path).EnumerateDirectories(searchPattern, searchOption).Select(FileSystemExtensions.CreateDetail),
            _ => throw new ArgumentOutOfRangeException(nameof(entryOption)),
            };

    /// <summary>
    /// Returns an enumerable collection of file and/or directory names that match a search pattern in a specified path,
    /// and optionally searches subdirectories.
    /// </summary>
    public static IEnumerable<string> EnumerateEntriesName(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, EntryOption entryOption = EntryOption.All)
        => entryOption switch
            {
            EntryOption.All => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption),
            EntryOption.Files => Directory.EnumerateFiles(path, searchPattern, searchOption),
            EntryOption.Directories => Directory.EnumerateDirectories(path, searchPattern, searchOption),
            _ => throw new ArgumentOutOfRangeException(nameof(entryOption)),
            };

    /// <summary>
    /// Copia lineas de un archivo de texto a otro, que satisfacen una condición.
    /// </summary>
    /// <remarks>
    /// Las lineas son anexadas al archivo destino.
    /// </remarks>
    public static void CopyLines(string inputFilename, string outputFilename, Func<string, bool> predicate) => File.AppendAllLines(
            outputFilename,
            File.ReadLines(inputFilename).Where(predicate));

    /// <summary>
    /// Determines whether the specified file or directory exists.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns>true if the specified file or directory exists.</returns>
    public static bool EntryExists(string path)
        => File.Exists(path) || Directory.Exists(path);

    /// <summary>
    /// Dado un nombre de origen genera uno de destino que no exista.
    /// </summary>
    /// <param name="sourceFullName">El path del archivo o directorio de origen.</param>
    /// <param name="destFullName">
    ///     El path de la nueva ubicacion para sourceFullName.
    ///     Si sourceFullName es un archivo, estonces destFullName también debe ser un archivo.
    ///     destFullName puede ser un path relativo.
    /// </param>
    /// <param name="antiCollisionFilenameFormat">Formato de String.Format para generar un nuevo nombre de destino si la entrada ya existe, donde {0} hace referencia al nombre de destino propuesto originalmente (la extension se mantiene).</param>
    /// <param name="retryCount">Cantidad de reintentos si el destino existe. Se generan nuevos nombres aplicando el formato recursivamente.</param>
    /// <returns>El path completo generado.</returns>
    public static string CreateNewName(string sourceFullName, string destFullName, string antiCollisionFilenameFormat = "{0} (2)", int retryCount = 3)
        {
        if (retryCount <= 0)
            {
            return "";
            }

        string? sourceDirectoryName = Path.GetDirectoryName(sourceFullName);

        if (!Path.IsPathRooted(destFullName))
            {
            destFullName = Path.Combine(sourceDirectoryName ?? "", destFullName);
            }

        return FileSystemExtensions.EntryExists(destFullName)
            ? FileSystemExtensions.CreateNewName(
                sourceFullName,
                FileSystemExtensions.FormatName(destFullName, antiCollisionFilenameFormat),
                antiCollisionFilenameFormat,
                retryCount - 1)
            : destFullName;
        }

    /// <summary>
    /// Aplica un formato a un nombre de archivo.
    /// </summary>
    /// <param name="path">Nombre completo (path + nombre + ext) del archivo a tratar.</param>
    /// <param name="format">Formato a aplicar solo al nombre del archivo.</param>
    /// <remarks>
    /// El formato se aplica solo al nombre del archivo, el path y la extension son excluidos y conservados.
    /// El formato debe contener {0} que representa al nombre del archivo.
    /// </remarks>
    /// <returns>El nuevo nombre del archivo.</returns>
    public static string FormatName(string path, string format = "{0} (2)")
        => Path.Combine(Path.GetDirectoryName(path) ?? "", String.Concat(String.Format(format, Path.GetFileNameWithoutExtension(path)), Path.GetExtension(path)));

    /// <summary>
    /// Idem File.Move pero si el archivo ya existe en el destino lo mueve igual (sobreescribiendolo).
    /// </summary>
    /// <param name="sourceFileName"></param>
    /// <param name="destFileName"></param>
    public static void ForceMoveFile(string sourceFileName, string destFileName)
        {
        if(File.Exists(destFileName))
            {
            File.Delete(destFileName);
            }

        File.Move(sourceFileName, destFileName);
        }

    /// <summary>
    /// Mueve un archivo o directorio (y su contenido) a una nueva ubicación.
    /// </summary>
    /// <param name="sourceFullName">El path del archivo o directorio a mover.</param>
    /// <param name="destFullName">
    ///     El path de la nueva ubicacion para sourceFullName.
    ///     Si sourceFullName es un archivo, estonces destFullName también debe ser un archivo.
    ///     Si destFullName no es una ruta completa, devuelve por referencia el path completo generado.
    /// </param>
    /// <param name="antiCollisionFilenameFormat">Formato de String.Format para generar un nuevo nombre de destino si la entrada ya existe, donde {0} hace referencia al nombre de destino propuesto originalmente (la extension se mantiene).</param>
    /// <param name="retryCount">Cantidad de reintentos si el destino existe.</param>
    /// <param name="test">Si es true devuelve la referencia al nombre destino pero no realiza ninguna accion.</param>
    /// <returns>True si la operacion fue exitosa, false si no se realizó.</returns>
    public static bool MoveToNewEntry(string sourceFullName, ref string destFullName, string antiCollisionFilenameFormat = "{0} (2)", int retryCount = 3, bool test = false)
        {
        string newFullName = FileSystemExtensions.CreateNewName(sourceFullName, destFullName, antiCollisionFilenameFormat, retryCount);

        if (newFullName.Length == 0)
            {
            return false;
            }

        destFullName = newFullName;

        FileSystemExtensions.MoveEntry(sourceFullName, ref destFullName, test);

        return true;
        }

    /// <summary>
    /// Mueve un archivo o directorio (y su contenido) a una nueva ubicación.
    /// </summary>
    /// <param name="sourceFullName">El path del archivo o directorio a mover.</param>
    /// <param name="destFullName">
    ///     El path de la nueva ubicacion para sourceFullName.
    ///     Si sourceFullName es un archivo, estonces destFullName también debe ser un archivo.
    ///     Si destFullName no es una ruta completa, devuelve por referencia el path completo generado.
    /// </param>
    /// <param name="test">Si es true devuelve la referencia al nombre destino pero no realiza ninguna accion.</param>
    /// <remarks>
    /// Si un archivo de destino existe, lanza una excepción.
    ///
    /// Este metodo permite sobrellevar la siguiente limitacion:
    ///     Si se usa Directory.Move o y DirectoryInfo.MoveTo para renombrar directorios y el nuevo nombre solo difiere del anterior en las mayusculas/minusculas
    ///       (case) de los caracteres, se produce una excepcion.
    /// </remarks>
    /// <returns>True si la operacion fue exitosa, False si no se realizó.</returns>
    public static bool MoveEntry(string sourceFullName, ref string destFullName, bool test = false)
        {
        if(destFullName != "")
            {
            string sourceDirectoryName = Path.GetDirectoryName(sourceFullName) ?? "";

            if(!Path.IsPathRooted(destFullName))
                {
                destFullName = Path.Combine(sourceDirectoryName, destFullName);
                }

            if (!test && String.CompareOrdinal(sourceFullName, destFullName) != 0)
                {
                if (Directory.Exists(sourceFullName))
                    {
                    if (String.Equals(sourceFullName, destFullName, StringComparison.OrdinalIgnoreCase))
                        {
                        string tempFullName = Path.Combine(sourceDirectoryName, Guid.NewGuid().ToString());

                        Directory.Move(sourceFullName, tempFullName);
                        Directory.Move(tempFullName, destFullName);
                        }
                    else
                        {
                        Directory.Move(sourceFullName, destFullName);
                        }
                    }
                else
                    {
                    File.Move(sourceFullName, destFullName);
                    }

                return true;
                }
            }

        return false;
        }

    public static void MoveDirectoryTo(string sourcePath, string destPath) => new DirectoryInfo(sourcePath).MoveTo(destPath);

    /// <summary>
    /// Copia un archivo o directorio (y su contenido) a una nueva ubicación.
    /// </summary>
    /// <param name="sourceFullName">El path del archivo o directorio a mover.</param>
    /// <param name="destFullName">
    ///     El path de la nueva ubicacion para sourceFullName.
    ///     Si sourceFullName es un archivo, estonces destFullName también debe ser un archivo.
    ///     Si destFullName no es una ruta completa, devuelve por referencia el path completo generado.
    /// </param>
    /// <param name="antiCollisionFilenameFormat">Formato de String.Format para generar un nuevo nombre de destino si la entrada ya existe, donde {0} hace referencia al nombre de destino propuesto originalmente (la extension se mantiene).</param>
    /// <param name="retryCount">Cantidad de reintentos si el destino existe. Se generan nuevos nombres aplicando el formato recursivamente.</param>
    /// <param name="test">Si es true devuelve la referencia al nombre destino pero no realiza ninguna accion.</param>
    /// <returns>True si la operacion fue exitosa, false si no se realizó.</returns>
    public static bool CopyToNewEntry(string sourceFullName, ref string destFullName, string antiCollisionFilenameFormat = "{0} (2)", int retryCount = 3, bool test = false)
        {
        destFullName = FileSystemExtensions.CreateNewName(sourceFullName, destFullName, antiCollisionFilenameFormat, retryCount);

        if (test || destFullName.Length == 0)
            {
            return false;
            }

        FileSystemExtensions.CopyEntryCore(sourceFullName, destFullName);
        return true;
        }

    /// <summary>
    /// Copia un archivo o directorio (y su contenido) a una nueva ubicación.
    /// </summary>
    /// <param name="sourceFullName">El path del archivo o directorio a mover.</param>
    /// <param name="destFullName">
    ///     El path de la nueva ubicacion para sourceFullName.
    ///     Si sourceFullName es un archivo, estonces destFullName también debe ser un archivo.
    ///     Si destFullName no es una ruta completa, devuelve por referencia el path completo generado.
    /// </param>
    /// <param name="overwrite">Si es true sobreescribe los archivos existentes. Si es false y el archivo de destino existe lanza una excepción.</param>
    /// <param name="test">Si es true devuelve la referencia al nombre destino pero no realiza ninguna accion.</param>
    ///
    /// <returns>True si la operacion fue exitosa, False si no se realizó.</returns>
    public static bool CopyEntry(string sourceFullName, ref string destFullName, bool overwrite = false, bool test = false)
        {
        if (destFullName != "")
            {
            string sourceDirectoryName = Path.GetDirectoryName(sourceFullName) ?? "";

            if (!Path.IsPathRooted(destFullName))
                {
                destFullName = Path.Combine(sourceDirectoryName, destFullName);
                }

            if (!test && String.CompareOrdinal(sourceFullName, destFullName) != 0)
                {
                FileSystemExtensions.CopyEntryCore(sourceFullName, destFullName, overwrite);
                return true;
                }
            }

        return false;
        }

    /// <summary>
    /// Copia un archivo o directorio.
    /// </summary>
    /// <param name="sourceFullName">El path del archivo o directorio de origen.</param>
    /// <param name="destFullName">El path del archivo o directorio de destino.</param>
    /// <param name="overwrite">Si es true sobreescribe los archivos existentes.</param>
    public static void CopyEntryCore(string sourceFullName, string destFullName, bool overwrite = false)
        {
        if (Directory.Exists(sourceFullName))
            {
            FileSystemExtensions.CopyDirectoryTo(sourceFullName, destFullName, overwrite);
            }
        else
            {
            File.Copy(sourceFullName, destFullName, overwrite);
            }
        }

    /// <summary>
    /// Copia un archivo a una carpeta.
    /// </summary>
    /// <param name="sourceFileName">Nombre del archivo.</param>
    /// <param name="destPath">Carpeta de destino.</param>
    /// <param name="overwrite">True si el archivo debe ser sobreescrito.</param>
    /// <remarks>
    /// Si el nombre del archivo a compiar no es completo se busca en el directorio base del dominio.
    /// </remarks>
    public static void CopyFileTo(string sourceFileName, string destPath, bool overwrite = false) => File.Copy(sourceFileName, Path.Combine(destPath, sourceFileName), overwrite);

    public static void CopyDirectoryTo(string sourcePath, string destPath, bool overwrite = false) => FileSystemExtensions.CopyDirectoryTo(sourcePath, destPath, "*", overwrite);

    public static void CopyDirectoryTo(string sourcePath, string destPath, string searchPattern = "*", bool overwrite = false) => new DirectoryInfo(sourcePath).CopyTo(new DirectoryInfo(destPath), searchPattern, overwrite);

    public static void CopyTo(this DirectoryInfo sourceDirectoryInfo, string destPath, string searchPattern = "*", bool overwrite = false) => sourceDirectoryInfo.CopyTo(new DirectoryInfo(destPath), searchPattern, overwrite);

    /// <summary>
    /// Copia el directorio especificado como origen al especificado como destino.
    /// </summary>
    /// <param name="sourceDirectoryInfo">Directorio origen</param>
    /// <param name="destDirectoryInfo">Directorio destino</param>
    /// <param name="searchPattern">Patron de archivos a copiar.</param>
    /// <param name="overwrite">Si es true sobreescribe los archivos existentes.</param>
    /// <remarks>Si searchPattern es "" copia solo los directorios (no los archivos).</remarks>
    public static void CopyTo(this DirectoryInfo sourceDirectoryInfo, DirectoryInfo destDirectoryInfo, string searchPattern, bool overwrite)
        {
        if (!destDirectoryInfo.Exists)
            {
            destDirectoryInfo.Create();
            }

        // copy files 
        foreach (FileInfo fileInfo in sourceDirectoryInfo.GetFiles(searchPattern))
            {
            fileInfo.CopyTo(Path.Combine(destDirectoryInfo.FullName, fileInfo.Name), overwrite);
            }

        // copy directories 
        foreach (DirectoryInfo directoryInfo in sourceDirectoryInfo.GetDirectories())
            {
            directoryInfo.CopyTo(Path.Combine(destDirectoryInfo.FullName, directoryInfo.Name), searchPattern, overwrite);
            }
        }

    public static bool IsDirectory(this FileSystemInfo file)
        => file.IsFlagSet(FileAttributes.Directory);

    /// <summary>
    /// True si la bandera de FileAttributes está seteada.
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static bool IsFlagSet(this FileSystemInfo fileSystemInfo, FileAttributes attribute)
        => (fileSystemInfo.Attributes & attribute) == attribute;

    /// <summary>
    /// Setea una bandera de FileAttributes.
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public static void SetFlag(this FileSystemInfo fileSystemInfo, FileAttributes attribute, bool value)
        {
        if(value)
            {
            fileSystemInfo.Attributes |= attribute;
            }
        else
            {
            fileSystemInfo.Attributes &= ~attribute;
            }
        }
    }
