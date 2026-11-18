
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.FileSystem;

public static class PathExtensions
    {
    #region Summarize
    /// <summary>
    /// Abrevia un path, colocando ... antes en el medio
    /// </summary>
    /// <param name="strPath"></param>
    /// <returns></returns>
    public static string SummarizePath(this string strPath)
        => String.Concat(Path.GetDirectoryName(strPath)?.Summarize().TrimEnd(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, Path.GetFileName(strPath));

    /// <summary>
    /// Abrevia una cadena a la mitad, colocando ... al final
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Summarize(this string str)
        => str.Summarize(str.Length / 2);

    /// <summary>
    /// Abrevia una cadena si la longitud supera la especificada, colocando ... al final
    /// </summary>
    public static string Summarize(this string str, int minLength)
        => str.Summarize(minLength, "…");

    /// <summary>
    /// Abrevia una cadena si la longitud supera la especificada, colocando ... al final
    /// </summary>
    public static string Summarize(this string str, int minLength, string replacement)
        {
        int midLength = (minLength - replacement.Length)/2;
        return midLength <= 0 || str.Length <= minLength? str: String.Concat(str[..midLength], replacement, str[^midLength..]);
        }
    #endregion

    public static string[] SplitDirectoryPath(this string path)
        => path.Split(Path.DirectorySeparatorChar);

    public static string[] SplitUriPath(this string path)
        => path.Split('/');

    #region Path Validation
    /// <summary>
    /// Retorna true si el path es válido.
    /// </summary>
    /// <remarks>
    /// A diferencia GetInvalidFileNameChars y GetInvalidPathChars que permiten chequear la validez de un nombre de archivo o una ruta,
    /// respectiv e independientemente, con este metodo puede validarse de forma conjunta, es decir el path y el nombre de archivo de un nombre de archivo completo.
    /// </remarks>
    public static bool IsValid(this string path)
        {
        try
            {
            PathExtensions.ValidatePath(path);
            return true;
            }
        catch
            {
            return false;
            }
        }

    /// <summary>
    /// Lanza una excepción si el path es inválido.
    /// </summary>
    /// <remarks>
    /// A diferencia GetInvalidFileNameChars y GetInvalidPathChars que permiten chequear la validez de un nombre de archivo o una ruta,
    /// respectiv e independientemente, con este metodo puede validarse de forma conjunta, es decir el path y el nombre de archivo de un nombre de archivo completo.
    /// </remarks>
    public static void ValidatePath(this string path)
        {
        try
            {
            Path.GetFullPath(path);
            }
        catch(Exception e)
            {
            throw new ArgumentException(String.Concat(e.Message, " Path: ", path), e);
            }
        }
    #endregion

    #region InvalidChars
    #region FileName
    public static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    #region Contains
    public static bool ContainsInvalidFileNameChars(this string strTarget)
        => strTarget.ToCharArray().ContainsInvalidFileNameChars();

    public static bool ContainsInvalidFileNameChars(this IEnumerable<char> strTarget)
        => strTarget.Any(PathExtensions.InvalidFileNameChars.Contains);
    #endregion

    #region Remove
    public static string RemoveInvalidFileNameChars(this string strTarget)
        => strTarget.RemoveChars(PathExtensions.InvalidFileNameChars);
    #endregion

    #region Replace
    public static string ReplaceInvalidFileNameChars(this string strTarget)
        => strTarget.ReplaceInvalidFileNameChars('_');

    public static string ReplaceInvalidFileNameChars(this string strTarget, char newChar)
        => strTarget.ReplaceChars(PathExtensions.InvalidFileNameChars, newChar);
    #endregion
    #endregion

    #region Path
    public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

    #region Contains
    public static bool ContainsInvalidPathChars(this string strTarget)
        => strTarget.ToCharArray().ContainsInvalidPathChars();

    public static bool ContainsInvalidPathChars(this IEnumerable<char> strTarget)
        => strTarget.Any(PathExtensions.InvalidPathChars.Contains);
    #endregion

    #region Remove
    public static string RemoveInvalidPathChars(this string strTarget)
        => strTarget.RemoveChars(PathExtensions.InvalidPathChars);
    #endregion

    #region Replace
    public static string ReplaceInvalidPathChars(this string strTarget)
        => strTarget.ReplaceInvalidPathChars('_');

    public static string ReplaceInvalidPathChars(this string strTarget, char newChar)
        => strTarget.ReplaceChars(PathExtensions.InvalidPathChars, newChar);
    #endregion
    #endregion
    #endregion

    /// <summary>
    /// Combines the strings into a path.
    /// </summary>
    /// <param name="paths"></param>
    /// <returns>A string that contains the combined paths.</returns>
    public static string Combine(IEnumerable<string> paths)
        => paths.Aggregate("", Path.Combine);

    /// <summary>
    /// Returns the path and file name without the extension.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>
    ///     A String containing the path and file name,
    ///     minus the last period (.) and all characters following it.
    /// </returns>
    public static string GetFullPathWithoutExtension(this string path)
        => Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));


    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">
    /// Path that defines the start of the relative path.
    /// </param>
    /// <param name="toPath">
    /// Path that defines the endpoint of the relative path.
    /// </param>
    /// <returns>
    /// The relative path from the start path to the end path.
    /// </returns>
    /// <example>
    ///          [TestMethod]
    ///          public void CanGetRelativePath()
    ///              {
    ///              string path = PathExtensions.RelativePathTo(@"c:\toto\titi", @"c:\toto\titi\tutu.txt");
    ///              Assert.AreEqual("tutu.txt", path);
    ///              path = PathExtensions.RelativePathTo(@"c:\toto\tata", @"c:\toto\titi\tutu.txt");
    ///              Assert.AreEqual(@"..\titi\tutu.txt", path);
    ///              }
    /// </example>
    public static string RelativePathTo(string fromPath, string toPath)
        {
        if (Path.IsPathRooted(fromPath) &&
            Path.IsPathRooted(toPath) &&
            !String.Equals(Path.GetPathRoot(fromPath), Path.GetPathRoot(toPath), StringComparison.OrdinalIgnoreCase))
            {
            return toPath;
            }

        string[] fromDirectories = fromPath.Split(Path.DirectorySeparatorChar);

        string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);

        int length = Math.Min(fromDirectories.Length, toDirectories.Length);

        int lastCommonRoot = -1;

        // find common root
        for (int i = 0; i < length; i++)
            {
            if (!String.Equals(fromDirectories[i], toDirectories[i], StringComparison.OrdinalIgnoreCase))
                {
                break;
                }

            lastCommonRoot = i;
            }

        if (lastCommonRoot == -1)
            {
            return toPath;
            }

        List<string> relativePath = [];

        // add relative folders in from path
        for (int i = lastCommonRoot + 1; i < fromDirectories.Length; i++)
            {
            if (fromDirectories[i].Length > 0)
                {
                relativePath.Add("..");
                }
            }

        // add to folders to path
        for (int i = lastCommonRoot + 1; i < toDirectories.Length; i++)
            {
            relativePath.Add(toDirectories[i]);
            }

        // create relative path
        return String.Join(Path.DirectorySeparatorChar.ToString(), relativePath);
        }

    /// <summary>
    ///     Initializes a new instance of the System.Uri class based on the combination
    ///     of a specified base System.Uri instance and a relative System.Uri instance.
    /// </summary>
    /// <param name="baseUri">An absolute System.Uri that is the base for the new System.Uri instance.</param>
    /// <param name="relativeUri">A relative System.Uri instance that is combined with baseUri.</param>
    /// <returns>New instance of the System.Uri class.</returns>
    public static Uri CombineUri(this Uri baseUri, Uri? relativeUri)
        {
        if (relativeUri == null)
            {
            return baseUri;
            }

        if (relativeUri.IsAbsoluteUri)
            {
            return relativeUri;
            }

        if (!baseUri.AbsoluteUri.EndsWith('/'))
            {
            baseUri = new Uri(String.Concat(baseUri.AbsoluteUri, "/"));
            }

        return new Uri(baseUri, relativeUri);
        }
    }
