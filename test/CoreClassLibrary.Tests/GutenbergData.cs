
using CoreClassLibrary.FileSystem;
using Xunit;

namespace CoreClassLibrary.Tests;

public class GutenbergData
    {
    public static class BookId
        {
        public const int WarAndPeace = 2600;    // 3 MB
        public const int DonQuijote = 2000;     // 2 MB
        public const int TheIliad = 6130;       // 1 MB
        public const int MobyDick = 2701;       // 1 MB
        public const int Frankenstein = 84;    // 412 KB
        public const int RomeoJuliet = 1513;    // 150 KB
        //public const int Alice = 11;    
        }

    public static Task<string> GetWarAndPeaceAsync()
        => GutenbergData.GetBookAsync(BookId.WarAndPeace);

    public static Task<string> GetTheIliadAsync()
        => GutenbergData.GetBookAsync(BookId.TheIliad);

    public static Task<string> GetDonQuijoteAsync()
        => GutenbergData.GetBookAsync(BookId.DonQuijote);

    public static Task<string> GetBookAsync(int id, CancellationToken cancellationToken = default)
        => FileSystemExtensions.GetOrWriteAsync($"C:\\temp\\{id}.txt", cancellation => GutenbergData.DownloadBookAsync(id, cancellation), cancellationToken);

    public static async Task<string> DownloadBookAsync(int id, CancellationToken cancellationToken = default)
        {
        using HttpClient client = new HttpClient();
        return await client.GetStringAsync($"http://www.gutenberg.org/files/{id}/{id}-0.txt", cancellationToken);   // $"https://www.gutenberg.org/cache/epub/{id}/pg{id}.txt"
        }

    [Fact]
    public static Task DonQuijote()
        => GutenbergData.GetBookAsync(BookId.DonQuijote, TestContext.Current.CancellationToken);

    [Fact]
    public static Task TheIliad()
        => GutenbergData.GetBookAsync(BookId.TheIliad, TestContext.Current.CancellationToken);

    [Fact]
    public static Task MobyDick()
        => GutenbergData.GetBookAsync(BookId.MobyDick, TestContext.Current.CancellationToken);

    [Fact]
    public static Task RomeoJuliet()
        => GutenbergData.GetBookAsync(BookId.RomeoJuliet, TestContext.Current.CancellationToken);

    [Fact]
    public static Task WarAndPeace()
        => GutenbergData.GetBookAsync(BookId.WarAndPeace, TestContext.Current.CancellationToken);

    [Fact]
    public static Task Frankenstein()
        => GutenbergData.GetBookAsync(BookId.Frankenstein, TestContext.Current.CancellationToken);
    }
