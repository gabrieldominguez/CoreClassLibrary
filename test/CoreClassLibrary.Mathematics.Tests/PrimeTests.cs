#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Mathematics.Randomness;
using System.Numerics;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class PrimeTests(ITestOutputHelper output)
    {
    private void AssertPrime<TSelf>(TSelf candidate) where TSelf: IBinaryInteger<TSelf>
        {
        output.WriteLine(candidate.ToString() ?? "<null>");
        Assert.True(candidate.IsPrime());

        if (candidate != TSelf.CreateChecked(2))
            {
            Assert.False(candidate.IsEven());
            }
        }

    [Theory]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(17, true)]
    [InlineData(18, false)]
    [InlineData(19, true)]
    [InlineData(1, false)]  // by definition, 1 is not prime
    [InlineData(0, false)]  // 0 is not prime
    public void IsPrime(uint candidate, bool expected)
        {
        Assert.Equal(expected, candidate.IsPrime());

        if (expected && candidate != 2)
            {
            Assert.False(candidate.IsEven());
            }
        }

    [Theory]
    [InlineData(10, new int[] { 2, 3, 5, 7 })]
    [InlineData(20, new int[] { 2, 3, 5, 7, 11, 13, 17, 19 })]
    [InlineData(15, new int[] { 2, 3, 5, 7, 11, 13 })]
    [InlineData(30, new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 })]
    public void SieveOfEratosthenes(int limit, int[] expected)
        {
        Assert.True(Maths.AreEqual(expected, Primes.SieveOfEratosthenes(limit), 0));
        //Assert.Equal(expected, Maths.SieveOfEratosthenes(limit));
        }

    [Fact]
    public void SieveOfEratosthenesChecked()
        {
        foreach (int prime in Primes.SieveOfEratosthenes(RandomWrapper.Shared.Next(100)))
            {
            this.AssertPrime(prime);
            }
        }

    [Theory]
    [InlineData(0, new uint[] { })]
    [InlineData(1, new uint[] { 2 })]
    [InlineData(2, new uint[] { 2, 3 })]
    [InlineData(3, new uint[] { 2, 3, 7 })]
    [InlineData(4, new uint[] { 2, 3, 7, 43 })]
    public void EnumeratePrimesByEuclid(uint count, uint[] expected)
        {
        Assert.True(Maths.AreEqual(expected, Primes.EnumeratePrimesByEuclid(count), 0U));
        //Assert.Equal(expected, Maths.EnumeratePrimesByEuclid(count));
        }

    [Fact]
    public void EnumeratePrimesByEuclidChecked()
        {
        foreach (uint prime in Primes.EnumeratePrimesByEuclid(RandomWrapper.Shared.Next(100U)))
            {
            this.AssertPrime(prime);
            }
        }

    [Theory]
    [InlineData(0, new uint[] { })]
    [InlineData(1, new uint[] { })]
    [InlineData(2, new uint[] { 2 })]
    [InlineData(3, new uint[] { 3 })]
    [InlineData(4, new uint[] { 2, 2 })]
    [InlineData(6, new uint[] { 2, 3 })]
    [InlineData(84, new uint[] { 2, 2, 3, 7 })]
    [InlineData(360, new uint[] { 2, 2, 2, 3, 3, 5 })]
    [InlineData(864, new uint[] { 2, 2, 2, 2, 2, 3, 3, 3 })]
    public void PrimeFactorization(uint n, uint[] expected)
        {
        Assert.True(Maths.AreEqual(expected, n.PrimeFactorization(), 0U));
        //Assert.Equal(expected, n.PrimeFactorization());
        }

    [Fact]
    public void PrimeFactorizationChecked()
        {
        uint n = RandomWrapper.Shared.Next(1000U);
        output.WriteLine($"n = {n}");
        output.WriteLine("PrimeFactorization:");
        foreach (uint prime in n.PrimeFactorization())
            {
            this.AssertPrime(prime);
            }
        }

    /// <summary>
    /// Fundamental theorem of arithmetic
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Fundamental_theorem_of_arithmetic"/>
    /// <see cref="https://numerosprimos.org/teorema-de-euclides-primos-infinitos"/>
    [Fact]
    public void FundamentalTheoremOfArithmetic()
        {
        uint n = RandomWrapper.Shared.Next(1000U);
        output.WriteLine($"n = {n}");
        uint[] factors = n.PrimeFactorization().ToArray();
        output.WriteLine($"factors = {{ {string.Join(", ", factors)} }}");

        Assert.Equal(n, factors.Prod());
        }

    /// <summary>
    /// Goldbach's conjecture
    /// </summary>
    /// <remarks>Every even integer greater than 2 can be expressed as the sum of two prime numbers.</remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Goldbach%27s_conjecture"/>
    [Fact]
    public void GoldbachConjecture()
        {
        int limit = RandomWrapper.Shared.Next(1000);
        output.WriteLine($"limit = {limit}");        
        int[] primes = Primes.SieveOfEratosthenes(limit).ToArray();

        for (int n = 4; n <= limit; n += 2)
            {
            foreach (int p in primes)
                {
                if (p > n)
                    {
                    Assert.Fail();
                    }

                int q = n - p;
                if (primes.Contains(q))
                    {
                    output.WriteLine($"{n} = {p} + {q}");
                    break;
                    }
                }
            }
        }
    }
