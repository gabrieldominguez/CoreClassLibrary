#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Recursion;

/// <summary>
/// Recursive functions
/// </summary>
/// <see cref="https://plato.stanford.edu/entries/recursive-functions/"/>
/// <see cref="https://en.wikipedia.org/wiki/Recursion_(computer_science)"/>
public class RecursiveTests(ITestOutputHelper output)
    {
    private static int Fibonacci(int x)
        => x <= 1? x: Fibonacci(x - 2) + Fibonacci(x - 1);

    private static int Factorial(int x)
        => x <= 1? 1: x * Factorial(x - 1);

    private static int Summation(int x)
        => x == 0? 0: x + Summation(x - 1);

    private static int Sum(int x, int y)
        => y == 0? x: 1 + Sum(x, y - 1);

    private static int Prod(int x, int y)
        => y == 0? 0: x + Prod(x, y - 1);

    private static int Pow(int x, int y)
        => y == 0? 1: x * Pow(x, y - 1);

    private static double SuperPow(double x, double y)
        => y == 0? 1: x.Pow(SuperPow(x, y - 1));

    [Fact]
    public void TestFactorial()
        {
        int n = Random.Shared.Next(10);

        output.WriteLine(n.ToString());
        Assert.Equal(n.Fact(), Factorial(n));
        }

    [Fact]
    public void TestSummation()
        {
        int n = Random.Shared.Next(10);
        output.WriteLine(n.ToString());

        int sum = Summation(n);
        output.WriteLine(sum.ToString());

        Assert.Equal(Enumerable.Range(1, n).Sum(), sum);
        Assert.Equal(n * (n+1) / 2, sum);
        }

    [Fact]
    public void TestFibonacci()
        {
        int[] fibonacciSequence = [0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181];
        int n = Random.Shared.Next(fibonacciSequence.Length);

        output.WriteLine(n.ToString());
        Assert.Equal(fibonacciSequence[n], Fibonacci(n));
        }

    [Fact]
    public void TestSum()
        {
        int x = Random.Shared.Next(20);
        int y = Random.Shared.Next(20);

        output.WriteLine(x.ToString());
        output.WriteLine(y.ToString());
        Assert.Equal(x + y, Sum(x, y));
        }

    [Fact]
    public void TestProd()
        {
        int x = Random.Shared.Next(20);
        int y = Random.Shared.Next(20);

        output.WriteLine(x.ToString());
        output.WriteLine(y.ToString());
        Assert.Equal(x * y, Prod(x, y));
        }

    [Fact]
    public void TestPow()
        {
        int x = Random.Shared.Next(10);
        int y = Random.Shared.Next(10);

        output.WriteLine(x.ToString());
        output.WriteLine(y.ToString());
        Assert.Equal(Maths.Pow(x, (double)y), Pow(x, y));
        }

    [Theory]
    [InlineData(0, 0, 1)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 1)]
    [InlineData(2, 0, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 2, 4)]
    public void TestMathsPow(float x, float y, float expected)
        => Assert.Equal(expected, x.Pow(y));

    [Theory]
    [InlineData(0, 0, 1)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 1)]
    [InlineData(2, 0, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 2, 4)]
    public void TestRecursivePow(int x, int y, int expected)
        => Assert.Equal(expected, Pow(x, y));

    [Theory]
    [InlineData(0, 0, 1)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 2, 1)]
    [InlineData(0, 3, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 1)]
    [InlineData(1, 3, 1)]
    [InlineData(2, 0, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 2, 4)]
    [InlineData(2, 3, 16)]
    [InlineData(3, 0, 1)]
    [InlineData(3, 1, 3)]
    [InlineData(3, 2, 27)]
    public void TestSuperPow(int x, int y, int expected)
        => Assert.Equal(expected, SuperPow(x, y));

    [Fact]
    public void TestSuperPow3()
        {
        double x = 2;
        double superPow = SuperPow(x, 3);

        Assert.Equal(x.Pow(x.Pow(x)), superPow);
        Assert.Equal(16, superPow);
        }

    private static IEnumerable<uint> CollatzSequence(uint n)
        {
        while (n > 1)
            { 
            yield return n;
            n = Collatz(n);
            }

        yield return n;
        }

    private static uint RecursiveCollatz(uint n)
        => n > 1? RecursiveCollatz(Collatz(n)): n;

    private static uint Collatz(uint n)
        => n % 2 == 0? n / 2 : 3 * n + 1;

    /// <summary>
    /// Collatz conjecture test
    /// </summary>
    /// <param name="n"></param>
    /// <see cref="https://en.wikipedia.org/wiki/Collatz_conjecture"/>
    [Theory]
    [InlineData(3)]
    [InlineData(9)]
    [InlineData(12)]
    [InlineData(15)]
    [InlineData(19)]
    [InlineData(27)]
    [InlineData(39)]
    [InlineData(43)]
    [InlineData(45)]
    [InlineData(97)]
    public void TestCollatz(uint n)
        {
        foreach(uint c in CollatzSequence(n))
            { 
            output.WriteLine(c.ToString());
            }

        Assert.Equal(1U, RecursiveCollatz(n));
        }

    private static int Str2Num(string s)
        => s.Reverse().Index().Aggregate(0, (n, c) => n + (int) 10D.Pow(c.Index) * (c.Item - '0'));

    private static (int i, int n) RecursiveStr2Num(string s)
        {
        if (s == "")
            {
            return (0, 0);
            }
        else
            {
            (int i, int n) = RecursiveStr2Num(s[1..]);
            return (i+1, n + (int)10D.Pow(i) * (s[0] - '0'));
            }
        }

    [Fact]
    public void TestStr2Num()
        {
        int n = Random.Shared.Next();
        string s = n.ToString();
        output.WriteLine(s);
        Assert.Equal(n, Str2Num(s));
        Assert.Equal(n, RecursiveStr2Num(s).n);
        }

    private static T[] RecursiveReverse<T>(T[] source) 
        => source.Length == 1 ? source[0..1] : [.. RecursiveReverse(source[1..]), .. source[0..1]];

    [Fact]
    public void TestRecursiveReverse()
        {        
        string s = RandomWrapper.Shared.NextString(10);
        output.WriteLine(s);
        Assert.Equal(s.Reverse().ToArray(), RecursiveReverse(s.ToCharArray()));
        }
    }
