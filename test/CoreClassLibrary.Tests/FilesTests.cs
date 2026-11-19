
using Xunit;
using CoreClassLibrary.FileSystem;

namespace CoreClassLibrary.Tests;

public class FilesTests(ITestOutputHelper output)
    {
    [Fact]
    public void TestDirectoryWalker()
        {
        new DirectoryInfoWalker().Walk(dir => { output.WriteLine(dir.FullName); return true; });

        output.WriteLine("---");

        DirectoryWalker.Walk(dirName => { output.WriteLine(dirName); return true; });

        output.WriteLine("---");

        DirectoryWalker.Walk(dirName => { output.WriteLine(dirName); return false; });
        }

    [Theory]
    [InlineData(@"C:\temp\file.txt", "{0} (2)", @"C:\temp\file (2).txt")]
    public void FormatName(string path, string format, string expected)
        => Assert.Equal(expected, FileSystemExtensions.FormatName(path, format));

    [Theory]
    [InlineData(@"C:\temp\file.txt", "file")]
    [InlineData(@"C:\\temp\\letrasparavolar\\10.pdf", "10")]
    public void GetFileNameWithoutExtension(string path, string expected)
        => Assert.Equal(expected, Path.GetFileNameWithoutExtension(path));

    [Theory]
    [InlineData(@"C:\temp", " ( PDFDrive )", "")]
    [InlineData(@"C:\temp", "ConsoleApp1", "ConsoleApp2")]
    public void FilesRename(string path, string oldSubstring, string newSubstring)
        => new DirectoryInfo(path).RenameFiles(oldSubstring, newSubstring);

    [Theory]
    [InlineData(@"C:\temp", " ( PDFDrive )", "")]
    [InlineData(@"C:\temp", "App1", "App2")]
    public void FilesRenameTest2(string path, string oldSubstring, string newSubstring)
        {
        foreach (FileInfo file in new DirectoryInfo(path).EnumerateFiles(String.Concat("*", oldSubstring, "*")))
            {
            output.WriteLine("---");
            output.WriteLine("File fullName:"); output.WriteLine(file.FullName);

            file.Rename(oldSubstring, newSubstring);

            output.WriteLine("File new fullname:"); output.WriteLine(file.FullName);
            }
        }

    private const string Content = "{\"ApiClient\":{\"Uri\": \"http://nbsfdessrv02:2020\"}}";

    [Fact]
    public void RootedPath()
        {
        using DisposableFile file = new DisposableFile();
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), file.Path);

        Assert.False(Path.IsPathRooted("appsettings.json"));

        Assert.False(Path.IsPathRooted(file.Path));
        Assert.True(File.Exists(file.Path));

        Assert.True(Path.IsPathRooted(fullPath));
        Assert.True(File.Exists(fullPath));
        }

    [Fact]
    public void DeleteUnexistingFile() => File.Delete(Guid.NewGuid().ToString());

    [Fact]
    public void ReadUnexistingFile() => Assert.Throws<FileNotFoundException>(() => File.ReadAllText(Guid.NewGuid().ToString()));

    [Fact]
    public void DisposableFile()
        {
        DisposableFile file = new DisposableFile("appsettings-{0}.json", FilesTests.Content);

        using (file)
            {
            Assert.True(File.Exists(file.Path));

            Assert.Equal(FilesTests.Content, File.ReadAllText(file.Path));
            }

        Assert.False(File.Exists(file.Path));
        }

    [Fact]
    public void DisposableFileContent()
        {
        DisposableFile emptyFile = new DisposableFile();
        Assert.True(File.Exists(emptyFile.Path));
        }

    [Fact]
    public async Task FileChangeWatcher()
        {
        using DisposableFile file = new DisposableFile();

        string fullname = Path.Combine(Directory.GetCurrentDirectory(), file.Path);

        output.WriteLine(file.Path);
        output.WriteLine(Directory.GetCurrentDirectory());
        output.WriteLine(fullname);
        output.WriteLine(Path.GetDirectoryName(fullname) ?? "<null>");
        output.WriteLine(Path.GetFileName(fullname));

        Assert.True(File.Exists(file.Path));
        Assert.True(File.Exists(fullname));

        bool modified = false;
        using FileSystemWatcher watcher = FileSystemExtensions.CreateFileChangeWatcher(
            file.Path,
            (sender, e) => modified = true
            );

        Assert.False(modified);

        await File.WriteAllTextAsync(file.Path, FilesTests.Content, TestContext.Current.CancellationToken);

        await Task.Delay(1, TestContext.Current.CancellationToken);

        Assert.True(modified);
        }


    [Fact]
    public void DirectoryName()
        {
        Assert.Equal("C:\\TEMP", Path.GetDirectoryName("C:\\TEMP\\FOLDER"));
        }

    [Fact]
    public void RootDirectoryName()
        {
        Assert.Equal("C:\\", Path.GetDirectoryName("C:\\file.ext"));
        }

    [Fact]
    public void GetFullPathWithoutExtension()
        {
        Assert.Equal("C:\\file", "C:\\file.ext".GetFullPathWithoutExtension());
        Assert.Equal("file", "file.ext".GetFullPathWithoutExtension());
        }
    }
