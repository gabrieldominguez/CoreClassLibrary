
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.Objects.Pooling;
using System.Text;
using Xunit;

namespace CoreClassLibrary.Tests;

/*
Program testing can be used to show the presence of bugs, but never to show their absence.
Edsger Dijkstra 1970, p.7

Una buena prueba vale mil opiniones de expertos.
Wernher von Braun (1912-1977), ingeniero mecánico y aeroespacial alemán.

He aprendido a usar la palabra imposible con la mayor precaución.
Wernher von Braun (1912-1977), ingeniero mecánico y aeroespacial alemán.

God is in the details.
Ludwig mies van der Rohe, architect.
*/
/// <summary>
/// General Unit Tests
/// </summary>
/// <remarks>
/// The Arrange, Act, Assert(AAA) style:
/// * Arrange—Define all the parameters and create the class under test
/// * Act—Execute the method being tested and capture the result
/// * Assert—Verify that the result of the Act stage had the expected value
/// </remarks>
/// <see cref="https://blog.cleancoder.com/uncle-bob/2014/05/14/TheLittleMocker.html"/>
/// <see cref="https://blog.cleancoder.com/uncle-bob/2013/09/23/Test-first.html"/>
/// <see cref="https://xunit.net/docs/capturing-output"/>
/// <see cref="https://xunit.net/docs/shared-context"/>
public class UnitTests
    {
    [Fact]
    public void CombineActionsOfT()
        {
        static void addA(StringBuilder strB) => strB.Append('A');
        static void addB(StringBuilder strB) => strB.Append('B');

        Action<StringBuilder>? addAB = EnumerableExtensions.AsEnumerable(addA, addB).Combine();

        using PooledStringBuilder builder = new PooledStringBuilder();

        Assert.NotNull(addAB);
        addAB(builder.StringBuilder);

        Assert.Equal("AB", builder.StringBuilder.ToString());
        }

    [Fact]
    public void Union1()
        {
        Union<int, string> result = new("hello");

        string description = result.Match(
            i => $"int {i}",
            s => $"string {s}"
            );

        Assert.Equal("string hello", description);
        }

    [Fact]
    public void Union2()
        {
        Union<int, string> result = new(3);

        string description = result.Match(
            i => $"int {i}",
            s => $"string {s}"
            );

        Assert.Equal("int 3", description);
        }
    }
