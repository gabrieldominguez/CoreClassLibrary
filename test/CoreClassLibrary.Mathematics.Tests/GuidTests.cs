using CoreClassLibrary.Mathematics.Cryptography;
using CoreClassLibrary.Mathematics.Randomness;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

/// <summary>
/// GUID / UUID tests
/// </summary>
/// <see cref="http://www.ietf.org/rfc/rfc4122.txt"/>
/// <see cref="https://en.wikipedia.org/wiki/Universally_unique_identifier"/>
/// <see cref="https://www.uuidgenerator.net/"/>
/// <seealso cref="https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/guid.cs#L892"/>
/// <seealso cref="https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Guid.cs#L730"/>
/// <seealso cref="https://stackoverflow.com/questions/417108/why-are-there-dashes-in-a-net-guid/3557678#3557678"/>
public class GuidTests(ITestOutputHelper output)
    {
    [Fact]
    public void NewGuidToString()
        {
        Guid guid = Guid.NewGuid();

        output.WriteLine(guid.ToString());
        output.WriteLine(guid.ToString(""));
        output.WriteLine("D");
        output.WriteLine(guid.ToString("D"));
        output.WriteLine("");
        output.WriteLine("N");
        output.WriteLine(guid.ToString("N"));
        output.WriteLine("");
        output.WriteLine("B");
        output.WriteLine(guid.ToString("B"));
        output.WriteLine("P");
        output.WriteLine(guid.ToString("P"));
        output.WriteLine("");
        output.WriteLine("X");
        output.WriteLine(guid.ToString("X"));
        }

    [Theory]
    [InlineData("bbd3fa88-4546-4d75-b0e2-a2106e64031e")]
    [InlineData("bbd3fa8845464d75b0e2a2106e64031e")]
    [InlineData("{bbd3fa88-4546-4d75-b0e2-a2106e64031e}")]
    [InlineData("(bbd3fa88-4546-4d75-b0e2-a2106e64031e)")]
    public void GuidParse(string input)
        {
        output.WriteLine(Guid.Parse(input).ToString());
        output.WriteLine(Guid.Parse(input.ToUpperInvariant()).ToString().ToUpperInvariant());
        }

    [Theory]
    [InlineData("bbd3fa88-4546-4d75-b0e2-a2106e64031e")]
    public void GuidParseExactD(string input)
        {
        output.WriteLine(Guid.ParseExact(input, "D").ToString());
        output.WriteLine(Guid.ParseExact(input.ToUpperInvariant(), "D").ToString().ToUpperInvariant());
        }

    [Theory]
    [InlineData("a20ae8f9654a4146aae1f2d56289cef6")]
    [InlineData("bb10fb5be74b45358de031ca1ef2f641")]
    [InlineData("e794c67f4b154b9585932be4a68a1b3c")]
    public void GuidParseExactN(string input)
        {
        output.WriteLine(Guid.ParseExact(input, "N").ToString());
        output.WriteLine(Guid.ParseExact(input.ToUpperInvariant(), "N").ToString().ToUpperInvariant());
        }

    [Fact]
    public void NamespaceGuidTest0()
        {
        string name = RandomWrapper.Shared.NextString(10);
        this.NamespaceGuidTest1(name);
        }

    [Theory]
    [InlineData("LoremIpsum")]
    [InlineData("")]
    public void NamespaceGuidTest1(string name)
        {
        Guid guid = NamespaceGuid.Create(name);
        output.WriteLine(guid.ToString());
        this.NamespaceGuidTest2(name, guid.ToString());
        }

    [Theory]
    [InlineData("LoremIpsum", "0a870853-e38d-5c14-9a34-75d4a222a850")]
    [InlineData("", "0a68eb57-c88a-5f34-9e9d-27f85e68af4f")]
    public void NamespaceGuidTest2(string name, string guid) 
        => Assert.Equal(Guid.Parse(guid), NamespaceGuid.Create(name));
    }
