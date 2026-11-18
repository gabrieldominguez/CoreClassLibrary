using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.FileSystem;

/// <summary>
/// Visitor Pattern, para recorrer un arbol de directorios.
/// </summary>
public static class DirectoryWalker
    {
    public static bool Walk(Predicate<string> visitor)
        => DirectoryWalker.Walk(Directory.GetCurrentDirectory(), visitor);

    /// <summary>
    /// Recorre el arbol de subdirectorios del directorio y los pasa por parametro a un metodo del tipo Visitor
    /// </summary>
    /// <remarks>Si el metodo devuelve false, se detiene la exploración.</remarks>
    /// <param name="path">Directorio a recorrer.</param>
    /// <param name="visitor">Metodo para procesar cada item.</param>
    /// <returns>True si se recorrieron todos los directorios del arbol (False si se detuvo antes). </returns>
    public static bool Walk(string path, Predicate<string> visitor)
        => path.Walk(visitor, Directory.GetDirectories);
    }
