using Microsoft.IO;

namespace CoreClassLibrary.Streams;
/// <summary>
/// RecyclableMemoryStream Manager
/// </summary>
/// <remarks>
/// RecyclableMemoryStream
/// A library to provide pooling for .NET MemoryStream objects to improve application performance, especially in the area of garbage collection.
/// 
/// Note that RecyclableMemoryStreamManager should be declared once and it will live for the entire process lifetime. 
/// It is perfectly fine to use multiple pools if you desire, especially if you want to configure them differently.
/// </remarks>
/// <see cref="https://andrewlock.net/returning-xml-from-minimal-apis-in-dotnet-6/"/>
/// <see cref="https://www.infoq.com/news/2015/03/Memory-Stream/"/>
/// <see cref="MemoryStream"/>
/// <see cref="https://github.com/Microsoft/Microsoft.IO.RecyclableMemoryStream"/>
public class MemoryStreamManager
    {
    public static readonly RecyclableMemoryStreamManager Instance = new RecyclableMemoryStreamManager();
    }
