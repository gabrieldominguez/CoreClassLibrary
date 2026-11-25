
using CoreClassLibrary.Objects.Pooling;
using System.Numerics;

namespace CoreClassLibrary.Mathematics;

public static class Primes
    {
    /// <summary>
    /// Retorna true si el numero es primo.
    /// </summary>
    /// <see cref="https://numerosprimos.org/"/>
    /// <see cref="https://en.wikipedia.org/wiki/Prime_number"/>
    /// <see cref="https://en.wikipedia.org/wiki/Prime_number#Primality_of_one"/>
    public static bool IsPrime<TSelf>(this TSelf candidate) where TSelf: IBinaryInteger<TSelf>
        {
        if (candidate == TSelf.One) { return false; } // by definition, 1 is not prime

        TSelf two = TSelf.CreateChecked(2);
        if (candidate % two != TSelf.Zero)
            {
            TSelf limit = candidate.IntegerSqrt();

            for (TSelf divisor = two + TSelf.One; divisor <= limit; divisor += two)
                {
                if (candidate % divisor == TSelf.Zero)
                    {
                    return false;
                    }
                }

            return true;
            }

        return candidate == two;
        }
    
        
    /// <summary>
    /// Sieve of Eratosthenes
    /// </summary>
    /// <remarks>
    /// The sieve of Eratosthenes is an ancient algorithm for finding all prime numbers up to any given limit.
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes"/>
    /// <see cref="https://numerosprimos.org/criba-de-eratostenes/"/>
    public static IEnumerable<int> SieveOfEratosthenes(int limit)
        {
        if (limit < 2)
            {
            yield break;
            }

        using PooledMemory<bool> isPrime = new PooledMemory<bool>(limit + 1);
        isPrime.Memory.Span.Fill(true);

        isPrime.Memory.Span[0] = isPrime.Memory.Span[1] = false;

        for (int p = 2; p.Pow2() <= limit; p++)
            {
            if (isPrime.Memory.Span[p])
                {
                for (int multiple = p.Pow2(); multiple <= limit; multiple += p)
                    {
                    isPrime.Memory.Span[multiple] = false;
                    }
                }
            }

        for (int i = 2; i <= limit; i++)
            {
            if (isPrime.Memory.Span[i])
                {
                yield return i;
                }
            }    
        }

    /// <summary>
    /// Euclid’s Theorem Proof (Infinitely Many Primes)
    /// </summary>
    /// <remarks>
    /// Given a finite list of primes, returns a new prime
    /// not in the list, using Euclid's construction.
    ///
    /// Euclid’s Theorem: There are infinitely many primes. 
    /// Proof: given any finite list of primes, multiply them together and add 1. 
    /// The result is either prime itself, or divisible by a prime not in the list.
    /// 
    /// This doesn’t generate primes in order (like a sieve does).
    /// Instead, it demonstrates Euclid’s theorem: no finite prime list is complete.
    /// Each call gives you at least one prime not in your starting list.
    /// <see cref="SieveOfEratosthenes"/>
    /// <see cref="https://en.wikipedia.org/wiki/Euclid%27s_theorem"/>
    public static TSelf NextPrime<TSelf>(this IEnumerable<TSelf> primes) where TSelf : IBinaryInteger<TSelf>
        {
        TSelf product = primes.Prod();
        TSelf candidate = product + TSelf.One;

        // Simple primality check
        for (TSelf i = TSelf.CreateChecked(2); i.Pow2() <= candidate; i++)
            {
            if (candidate % i == TSelf.Zero)
                {
                return i; // a new prime divisor not in knownPrimes
                }
            }

        return candidate; // itself prime
        }

    public static IEnumerable<TSelf> EnumeratePrimesByEuclid<TSelf>(TSelf count) where TSelf : IBinaryInteger<TSelf>
        {
        List<TSelf> primes = [];

        for (TSelf i = TSelf.AdditiveIdentity; i < count; i++)
            {
            TSelf next = primes.NextPrime();
            primes.Add(next);
            yield return next;
            }
        }

    public static IEnumerable<TSelf> PrimeFactorization<TSelf>(this TSelf n) where TSelf : IBinaryInteger<TSelf>
        {
        if (n == TSelf.One) // by definition, 1 is not prime
            {
            yield break;
            }

        // Divide out small primes
        for (TSelf p = TSelf.CreateChecked(2); p.Pow2() <= n; p++)
            {
            while (n % p == TSelf.Zero)
                {
                yield return p;
                n /= p;
                }
            }

        // If n > 1, it’s a prime factor itself
        if (n > TSelf.One)
            {
            yield return n;
            }
        }
    }
