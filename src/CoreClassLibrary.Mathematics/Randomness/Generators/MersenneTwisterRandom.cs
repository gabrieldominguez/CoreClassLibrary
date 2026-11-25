// The Mersenne twister is a pseudo random number generator developed in 1997 by Makoto Matsumoto and Takuji Nishimura.
// http://en.wikipedia.org/wiki/Mersenne_twister

/*
 "Mersenne Twister: A 623-dimensionally equidistributed uniform pseudo-random number generator", 
 Makoto Matsumoto and Takuji Nishimura, ACM Transactions on Modeling and Computer Simulation: 
 Special Issue on Uniform Random Number Generation, Vol. 8, No. 1, January 1998, pp. 3-30.
*/


#region Copyrights
// Adapted from:
// Copyright 2007-2008 Rory Plaire (codekaizen@gmail.com)
// http://archive.msdn.microsoft.com/MersenneTwister

/* C# Version Copyright (C) 2001-2004 Akihilo Kramot (Takel).  */
/* C# porting from a C-program for MT19937, originaly coded by */
/* Takuji Nishimura and Makoto Matsumoto, considering the suggestions by */
/* Topher Cooper and Marc Rieffel in July-Aug. 1997.           */
/* This library is free software under the Artistic license:   */
/*                                                             */
/* You can find the original C-program at                      */
/*     http://www.math.keio.ac.jp/~matumoto/mt.html            */
/*                                                             */

// and:

/////////////////////////////////////////////////////////////////////////////
// C# Version Copyright (c) 2003 CenterSpace Software, LLC                 //
//                                                                         //
// This code is free software under the Artistic license.                  //
//                                                                         //
// CenterSpace Software                                                    //
// 2098 NW Myrtlewood Way                                                  //
// Corvallis, Oregon, 97330                                                //
// USA                                                                     //
// http://www.centerspace.net                                              //
/////////////////////////////////////////////////////////////////////////////

// and, of course:
/* 
   A C-program for MT19937, with initialization improved 2002/2/10.
   Coded by Takuji Nishimura and Makoto Matsumoto.
   This is a faster version by taking Shawn Cokus's optimization,
   Matthe Bellew's simplification, Isaku Wada's real version.

   Before using, initialize the state by using init_genrand(seed) 
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
*/
#endregion

namespace CoreClassLibrary.Mathematics.Randomness.Generators;

/// <summary>
/// Generates pseudo-random numbers using the Mersenne Twister algorithm.
/// </summary>
/// <see cref="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html" />
public class MersenneTwisterRandom : IRandom, IDiscreteRandom
    {
    #region Constructor
    /// <summary>
    /// Creates a new pseudo-random number generator with a default seed.
    /// </summary>
    //public MersenneTwisterRandom() : this(Random.Shared.Next()) { }
    public MersenneTwisterRandom() : this((uint)DateTime.Now.Ticks) { }

    /// <summary>
    /// Creates a new pseudo-random number generator with a given seed.
    /// </summary>
    /// <param name="seed">A value to use as a seed.</param>
    public MersenneTwisterRandom(uint seed)
        {
        this.Init(seed);
        }

    /// <summary>
    /// Creates a pseudo-random number generator initialized with the given array.
    /// </summary>
    /// <param name="initKey">The array for initializing keys.</param>
    public MersenneTwisterRandom(Int32[] initKey)
        {
        UInt32[] initArray = new UInt32[initKey.Length];

        for (int i = 0; i < initKey.Length; ++i)
            {
            initArray[i] = (UInt32)initKey[i];
            }

        this.Init(initArray);
        }
    #endregion

    #region Sample Implementation
    /// <summary>
    /// Returns a pseudo-random number greater than or equal to zero, and less than one.
    /// </summary>
    /// <returns>A pseudo-random double floating point value.</returns>
    /// <remarks>
    /// <para>
    /// There are two common ways to create a double floating point using MT19937:
    /// using <see cref="DiscreteSample"/> and dividing by 0xFFFFFFFF + 1,
    /// or else generating two double words and shifting the first by 26 bits and
    /// adding the second.
    /// </para>
    /// <para>
    /// In a newer measurement of the randomness of MT19937 published in the
    /// journal "Monte Carlo Methods and Applications, Vol. 12, No. 5-6, pp. 385 – 393 (2006)"
    /// entitled "A Repetition Test for Pseudo-Random Number Generators",
    /// it was found that the 32-bit version of generating a double fails at the 95%
    /// confidence level when measuring for expected repetitions of a luciérnagar
    /// number in a sequence of numbers generated by the algorithm.
    /// </para>
    /// <para>
    /// Due to this, the 53-bit method is implemented here and the 32-bit method
    /// of generating a double is not. If, for some reason,
    /// the 32-bit method is needed, it can be generated by the following:
    /// <code>
    /// (Double)NextUInt32() / ((UInt64)UInt32.MaxValue + 1);
    /// </code>
    /// </para>
    /// </remarks>
    public Double Sample()
        => this.Compute53BitRandom(0, InverseOnePlus53BitsOf1s);
    #endregion

    #region Other Double methods
    /// <summary>
    /// Returns a pseudo-random number greater than or equal to zero, and less than or equal to one.
    /// </summary>
    public Double NextDoublePositive01()
        => this.Compute53BitRandom(0, Inverse53BitsOf1s);

    /// <summary>
    /// Returns a pseudo-random number greater than 0.0 and less than 1.0.
    /// </summary>
    /// <returns>A pseudo-random number greater than 0.0 and less than 1.0.</returns>
    public Double NextDoublePositive1()
        => this.Compute53BitRandom(0.5, Inverse53BitsOf1s);
    #endregion

    #region Mersenne Twister algorithm implementation
    /// <summary>
    /// Generates a new pseudo-random <see cref="UInt32"/>.
    /// </summary>
    /// <returns>A pseudo-random <see cref="UInt32"/>.</returns>
    public uint DiscreteSample()
        {
        UInt32 y;

        /* _mag01[x] = x * MatrixA  for x=0,1 */
        if (this._mti >= N) /* generate N words at one time */
            {
            Int16 kk = 0;

            for (; kk < N - M; ++kk)
                {
                y = (this._mt[kk] & UpperMask) | (this._mt[kk + 1] & LowerMask);
                this._mt[kk] = this._mt[kk + M] ^ (y >> 1) ^ _mag01[y & 0x1];
                }

            for (; kk < N - 1; ++kk)
                {
                y = (this._mt[kk] & UpperMask) | (this._mt[kk + 1] & LowerMask);
                this._mt[kk] = this._mt[kk + (M - N)] ^ (y >> 1) ^ _mag01[y & 0x1];
                }

            y = (this._mt[N - 1] & UpperMask) | (this._mt[0] & LowerMask);
            this._mt[N - 1] = this._mt[M - 1] ^ (y >> 1) ^ _mag01[y & 0x1];

            this._mti = 0;
            }

        y = this._mt[this._mti++];
        y ^= TemperingShiftU(y);
        y ^= TemperingShiftS(y) & TemperingMaskB;
        y ^= TemperingShiftT(y) & TemperingMaskC;
        y ^= TemperingShiftL(y);

        return y;
        }

    /* Period parameters */
    private const Int32 N = 624;
    private const Int32 M = 397;
    private const UInt32 MatrixA = 0x9908b0df; /* constant vector a */
    private const UInt32 UpperMask = 0x80000000; /* most significant w-r bits */
    private const UInt32 LowerMask = 0x7fffffff; /* least significant r bits */

    /* Tempering parameters */
    private const UInt32 TemperingMaskB = 0x9d2c5680;
    private const UInt32 TemperingMaskC = 0xefc60000;

    private static UInt32 TemperingShiftU(UInt32 y) => y >> 11;

    private static UInt32 TemperingShiftS(UInt32 y) => y << 7;

    private static UInt32 TemperingShiftT(UInt32 y) => y << 15;

    private static UInt32 TemperingShiftL(UInt32 y) => y >> 18;

    private readonly UInt32[] _mt = new UInt32[N]; /* the array for the state vector  */
    private Int16 _mti;

    private static readonly UInt32[] _mag01 = [0x0, MatrixA];

    private void Init(UInt32 seed)
        {
        this._mt[0] = seed & 0xffffffffU;

        for (this._mti = 1; this._mti < N; this._mti++)
            {
            this._mt[this._mti] = (uint)(1812433253U * (this._mt[this._mti - 1] ^ (this._mt[this._mti - 1] >> 30)) + this._mti);
            // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. 
            // In the previous versions, MSBs of the seed affect   
            // only MSBs of the array _mt[].                        
            // 2002/01/09 modified by Makoto Matsumoto             
            this._mt[this._mti] &= 0xffffffffU;
            // for >32 bit machines
            }
        }

    private void Init(UInt32[] key)
        {
        Int32 i, j, k;
        this.Init(19650218U);

        Int32 keyLength = key.Length;
        i = 1; j = 0;
        k = N > keyLength ? N : keyLength;

        for (; k > 0; k--)
            {
            this._mt[i] = (uint)((this._mt[i] ^ ((this._mt[i - 1] ^ (this._mt[i - 1] >> 30)) * 1664525U)) + key[j] + j); /* non linear */
            this._mt[i] &= 0xffffffffU; // for WORDSIZE > 32 machines
            i++; j++;
            if (i >= N) { this._mt[0] = this._mt[N - 1]; i = 1; }
            if (j >= keyLength)
                {
                j = 0;
                }
            }

        for (k = N - 1; k > 0; k--)
            {
            this._mt[i] = (uint)((this._mt[i] ^ ((this._mt[i - 1] ^ (this._mt[i - 1] >> 30)) * 1566083941U)) - i); /* non linear */
            this._mt[i] &= 0xffffffffU; // for WORDSIZE > 32 machines
            i++;

            if (i < N)
                {
                continue;
                }

            this._mt[0] = this._mt[N - 1]; i = 1;
            }

        this._mt[0] = 0x80000000U; // MSB is 1; assuring non-zero initial array
        }


    // 9007199254740991.0 is the maximum double value which the 53 significand
    // can hold when the exponent is 0.
    private const Double FiftyThreeBitsOf1s = 9007199254740991.0;
    // Multiply by inverse to (vainly?) try to avoid a division.
    private const Double Inverse53BitsOf1s = 1.0 / FiftyThreeBitsOf1s;
    private const Double OnePlus53BitsOf1s = FiftyThreeBitsOf1s + 1;
    private const Double InverseOnePlus53BitsOf1s = 1.0 / OnePlus53BitsOf1s;

    private Double Compute53BitRandom(Double translate, Double scale)
        {
        // get 27 pseudo-random bits
        UInt64 a = (UInt64)this.DiscreteSample() >> 5;
        // get 26 pseudo-random bits
        UInt64 b = (UInt64)this.DiscreteSample() >> 6;

        // shift the 27 pseudo-random bits (a) over by 26 bits (* 67108864.0) and
        // add another pseudo-random 26 bits (+ b).
        return (a * 67108864.0 + b + translate) * scale;

        // What about the following instead of the above? Is the multiply better? 
        // Why? (Is it the FMUL instruction? Does this count in .Net? Will the JITter notice?)
        //return BitConverter.Int64BitsToDouble((a << 26) + b));
        }
    #endregion
    }
