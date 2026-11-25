
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Recursion;

/// <summary>
/// Iterative version of recursive functions
/// </summary>
/// <see cref="https://plato.stanford.edu/entries/recursive-functions/"/>
/// <see cref="https://www.geeksforgeeks.org/dsa/tail-recursion/"/>
/// <see cref="https://stackoverflow.com/questions/33923/what-is-tail-recursion"/>
public class IterativeTests(ITestOutputHelper output)
    {
    private static int Fibonacci(int x)
        {
        if (x <= 1)
            {
            return x;
            }

        int a = 0;
        int b = 1;

        for (int i = 2; i <= x; i++)
            {
            int temp = a + b;
            a = b;
            b = temp;
            }

        return b;
        }

    private static int Factorial(int x)
        {
        int result = 1;

        for (int i = 2; i <= x; i++)
            {
            result *= i;
            }

        return result;
        }

    private static int Summation(int x)
        {
        int result = 0;

        for (int i = 1; i <= x; i++)
            {
            result += i;
            }

        return result;
        }

    private static int Sum(int x, int y)
        {
        int result = x;

        for (int i = 0; i < y; i++)
            {
            result++;
            }

        return result;
        }

    private static int Prod(int x, int y)
        {
        int result = 0;

        for (int i = 0; i < y; i++)
            {
            result += x;
            }

        return result;
        }

    private static int Pow(int x, int y)
        {
        int result = 1;

        for (int i = 0; i < y; i++)
            {
            result *= x;
            }

        return result;
        }

    public static double SuperPow(double x, double y)
        {
        double result = 1;

        for (int i = 0; i < y; i++)
            {
            result = x.Pow(result);
            }

        return result;
        }


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
    }
