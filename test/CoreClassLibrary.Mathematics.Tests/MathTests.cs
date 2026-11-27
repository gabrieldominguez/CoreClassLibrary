using CoreClassLibrary.Mathematics.Randomness;
using System.Text.Json;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class MathTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData("400638133393", '1')]   // EAN-13
    [InlineData("978059651774", '8')]
    [InlineData("590123412345", '7')]   
    [InlineData("7351353", '7')]    // EAN-8
    public void GetEAN13CheckDigit(string number, char expected)
        {
        Assert.Equal(expected, number.GetEANCheckDigit());
        }

    /// <summary>
    /// Compute Golden Ratio
    /// </summary>
    [Theory]
    [InlineData(1, false)]
    [InlineData(10, false)]
    [InlineData(12, false)]
    [InlineData(15, true)]
    [InlineData(20, true)]
    /// <see cref="https://www.mathsisfun.com/numbers/golden-ratio.html"/>
    /// <see cref="https://en.wikipedia.org/wiki/Golden_ratio"/>
    public void ComputeGoldenRatio(int n, bool equal)
        {
        double phi = 2;
        for (int i = 0; i < n; i++) 
            {
            phi = 1.0 / phi + 1.0;
            }
        
        output.WriteLine(Maths.Phi.ToString());
        output.WriteLine(phi.ToString());
        
        Assert.True(Maths.AreEqual(Maths.Phi, phi, 0.000001) == equal);
        }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(4, 2)]
    [InlineData(9, 3)]
    [InlineData(25, 5)]
    [InlineData(100, 10)]
    [InlineData(2, 1.4142135623730951)]
    [InlineData(3, 1.7320508075688772)]
    [InlineData(0.25, 0.5)]
    [InlineData(0.01, 0.1)]
    // https://www.youtube.com/shorts/cqphaodDkqg
    public void ComputeSqrtHeron(double number, double expected)
        {
        double x = number;

        while (Math.Abs(Maths.Pow2(x) - number) > 1e-10)
            {
            x = (x + number / x) / 2.0;
            }

        Assert.Equal(expected, x, 10);
        Assert.Equal(Math.Sqrt(number), x, 10);
        }

    [Fact]
    public void CordobaRatio()
        {
        Assert.Equal(1.306562964, Maths.Cordoba, 8);
        }
   
    [Theory]
    [InlineData(0, 1)]    // 10^0 = 1
    [InlineData(1, 10)]   // 10^1 = 10
    [InlineData(2, 100)]  // 10^2 = 100
    [InlineData(3, 1000)] // 10^3 = 1000
    [InlineData(4, 10000)]
    [InlineData(-1, 0.1)] // 10^-1 = 0.1
    [InlineData(-2, 0.01)]
    [InlineData(-3, 0.001)]
    [InlineData(-4, 0.0001)]
    public void TenPow(int exponent, double expected)
        {
        Assert.Equal(expected, Maths.TenPow<double>(exponent), 10);
        }

    [Fact]
    public void TenPowChecked()
        {
        int exponent = RandomWrapper.Shared.Next(-10, 10);
        double result = Maths.TenPow<double>(exponent);
        Assert.Equal(Math.Pow(10, exponent), result, 10);
        output.WriteLine($"10^{exponent} = {result}");
        }

    [Theory]
    [InlineData((uint)0, (uint)0)]
    [InlineData((uint)1, (uint)1)]
    [InlineData((uint)4, (uint)2)]
    [InlineData((uint)15, (uint)3)] // floor(sqrt(15)) = 3
    [InlineData((uint)16, (uint)4)]
    [InlineData((uint)17, (uint)4)] // floor(sqrt(17)) = 4
    [InlineData((uint)81, (uint)9)]
    public void IntegerSqrt_Works_For_UInt(uint value, uint expected)
        {
        Assert.Equal(expected, value.IntegerSqrt());
        }

    [Theory]
    [InlineData((ulong)0, (ulong)0)]
    [InlineData((ulong)100, (ulong)10)]
    [InlineData((ulong)9999, (ulong)99)] // floor(sqrt(9999)) = 99
    public void IntegerSqrt_Works_For_ULong(ulong value, ulong expected)
        {
        Assert.Equal(expected, value.IntegerSqrt());
        }

    [Theory]
    // Case: square function f(x) = x^2
    [InlineData(0.0, 10.0, 25.0, 1e-6, 1e-3)]   // f(5) = 25
    [InlineData(0.0, 100.0, 49.0, 1e-6, 1e-3)]  // f(7) = 49
    [InlineData(0.0, 100.0, 1.0, 1e-6, 1e-3)]   // f(1) = 1
    public void DichotomicOptimization_FindsDoubleRootOfSquare(double minX, double maxX, double targetY, double precisionX, double precisionY)
        {
        double result = Maths.DichotomicOptimization(minX, maxX, targetY, Maths.Pow2, precisionX, precisionY);

        output.WriteLine($"Result: x = {result}, f(x) = {Maths.Pow2(result)}, targetY = {targetY}");

        Assert.Equal(targetY, Maths.Pow2(result), precisionY);
        }

    [Theory]
    // Case: square function f(x) = x^2
    [InlineData(0,  10, 25, 0, 1, 5)]  // f(5) = 25
    [InlineData(0, 100, 49, 0, 1, 7)]  // f(7) = 49
    [InlineData(0, 100, 50, 0, 1, 7)]  // closest int root ~7 (since 7*7=49)
    public void DichotomicOptimization_FindsIntRootOfSquare(int minX, int maxX, int targetY, int precisionX, int precisionY, int expected)
        {
        int result = Maths.DichotomicOptimization(minX, maxX, targetY, Maths.Pow2, precisionX, precisionY);

        output.WriteLine($"Result: x = {result}, f(x) = {Maths.Pow2(result)}, targetY = {targetY}");
        
        int actualY = Maths.Pow2(result);
        Assert.InRange(actualY, targetY-precisionY, targetY+precisionY);
        
        Assert.Equal(expected, result);
        }

    private readonly IReadOnlyList<int> sortedList = [ 1, 3, 5, 7, 9, 11, 13 ];

    [Theory]
    [InlineData(1, 0)]
    [InlineData(5, 2)]
    [InlineData(13, 6)]
    public void BinarySearch_FindsExistingElement_ReturnsIndex(int target, int expectedIndex)
        {
        Assert.Equal(expectedIndex, sortedList.BinarySearch(target));
        }

    [Theory]
    [InlineData(0, 0)]  // before first element
    [InlineData(2, 1)]  // between 1 and 3
    [InlineData(4, 2)]  // between 3 and 5
    [InlineData(14, 7)] // after last element
    public void BinarySearch_MissingElement_ReturnsBitwiseComplement(int target, int expectedInsertion)
    {
        int result = sortedList.BinarySearch(target);
        Assert.Equal(~expectedInsertion, result);
    }

    [Fact]
    public void BinarySearch_EmptyList_ReturnsBitwiseComplementZero()
    {
        int[] empty = [];
        int result = empty.BinarySearch(10);
        Assert.Equal(~0, result);
    }

    [Theory]
    [InlineData('0')]
    [InlineData('1')]
    [InlineData('2')]
    [InlineData('3')]
    [InlineData('4')]
    [InlineData('5')]
    [InlineData('6')]
    [InlineData('7')]
    [InlineData('8')]
    [InlineData('9')]
    [InlineData('A')]
    [InlineData('B')]
    [InlineData('C')]
    [InlineData('D')]
    [InlineData('E')]
    [InlineData('F')]
    public void HexDigit(char c)
        {
        Assert.True(c.IsHexDigit());
        Assert.Equal(c, c.FromHexDigit().ToHEXDigit());
        Assert.Equal(String.Concat("0", c.ToString()).FromHexString()[0], c.FromHexDigit());
        Assert.Equal(String.Concat("0", c.ToString()).FromHexPair(), c.FromHexDigit());
        }

    [Fact]
    public void Hex()
        {
        Assert.False('X'.IsHexDigit());
        Assert.Equal(10, 'A'.FromHexDigit());
        Assert.Equal(160, 'A'.FromHexDigit()*16);
        Assert.Equal(160, "A0".FromHexPair());
        }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(0, 1, 2)]
    [InlineData(2, 0, 3)]
    [InlineData(1, 1, 4)]
    [InlineData(0, 2, 5)]
    [InlineData(3, 0, 6)]
    [InlineData(2, 1, 7)]
    [InlineData(1, 2, 8)]
    [InlineData(0, 3, 9)]
    [InlineData(47, 32, 3192)]
    [InlineData(52, 1, 1432)]
    public void CantorPairing(int x, int y, int z)
        {
        Assert.Equal(z, Maths.CantorPairing(x, y));
        Assert.Equal((x, y), Maths.CantorPairingInverse((float)z));
        }

    /// <summary>
    /// Problema propuesto por E. de la Roche en 1512
    /// </summary>
    /// <remarks>
    /// Un hombre quiere comprar 20 animales por 20 francos, y el precio de los animales es, a saber: 
    /// bueyes a 5 francos cada uno, 
    /// cerdos a 2 francos cada uno y 
    /// corderos a ½ franco cada uno. 
    /// Se pregunta, ¿Cuántos bueyes, cerdos y corderos habrá en la compra?
    /// </remarks>
    /// <see cref="https://blogs.eitb.eus/ciencia/2023/05/09/mateadictos-un-problema-propuesto-por-e-de-la-roche-en-1512/"/>
    [Fact]
    public void Roche()
        {
        for (int bueyes = 0; bueyes < 4; bueyes++)
            {
            for (int cerdos = 0; cerdos < 10; cerdos++)
                {
                for (int corderos = 0; corderos < 20; corderos++)
                    {
                    if(bueyes+cerdos+corderos == 20 && (5 * bueyes)+(2*cerdos)+(0.5 * corderos) == 20)
                        { 
                        output.WriteLine("Solucion: {0} bueyes, {1} cerdos y {3} corderos", bueyes, cerdos, corderos);
                        return;
                        }
                    }
                }
            }

        output.WriteLine("Sin solución.");
        }

    /// <summary>
    /// El problema de las balas de cañón
    /// </summary>
    /// <remarks>
    /// Si en la época en la que se utilizaban bolas como proyectiles para los cañones, éstas se hubiesen almacenado formando una pirámide de base cuadrada, con 15 bolas de cañón de lado en el cuadrado de la base, ¿cuantas bolas de cañón habría en la pirámide?
    /// </remarks>
    /// <see cref="https://blogs.eitb.eus/ciencia/2017/05/09/mateadictos-el-problema-de-las-balas-de-canon/amp/"/>
    [Fact]
    public void CannonBalls()
        {
        Assert.Equal(1240, Enumerable.Range(1, 15).Sum(Maths.Pow2));
        }

    /// <summary>
    /// Cantidad total de puntos en todas las fichas de un juego de domino.
    /// </summary>
    [Fact]
    public void DominoPoints()
        {
        int pointSum = 0;
        for(int i = 0; i < 7; i++)
            {
            for(int j = 0; j <= i; j++)
                {
                pointSum += i + j;
                }
            }
        
        Assert.Equal(168, pointSum);
        Assert.Equal(168, 12 + 2 * (21 + 27 + 30));
        Assert.Equal(168, Enumerable.Range(1, 6).Sum(i => 8 * i));
        Assert.Equal(168, 8 * Enumerable.Range(1, 6).Sum());
        }

    [Theory]
    [InlineData("[[-1,2,2,-24],[1,1,1,48],[2,-6,4,12]]", "[[-1,2,2,-24],[0,-3,-3,-24],[0,0,30,-60]]")]
    public void ComputeGaussianElimination(string matrixJson, string expectedMatrixJson)
        {
        double[][] matrix = JsonSerializer.Deserialize<double[][]>(matrixJson) ?? throw new ArgumentException("Invalid matrix format");

        Maths.ComputeGaussianElimination(matrix);

        output.WriteLine(JsonSerializer.Serialize(matrix));

        Assert.Equal(JsonSerializer.Deserialize<double[][]>(expectedMatrixJson), matrix);
        }    

   
    [Theory]
    [InlineData("[[1,2,3],[4,5,6]]", new double[] { -1, 2 })]
    [InlineData("[[1,1,50],[2,4,122]]", new double[] { 39, 11 })]
    [InlineData("[[2,2,100],[2,4,122]]", new double[] { 39, 11 })]
    [InlineData("[[1,3,14],[7,-1,10]]", new double[] { 2, 4 })]
    [InlineData("[[5,-1,24],[-1,1,24]]", new double[] { 12, 36 })]
    [InlineData("[[5,-1,24],[1,1,24]]", new double[] { 8, 16 })]
    [InlineData("[[3,-2,5,0],[4,5,8,1],[1,1,2,1]]", new double[] { -32, -3, 18 })]
    [InlineData("[[-1,2,2,-24],[1,1,1,48],[2,-6,4,12]]", new double[] { 40, 10, -2 })]
    [InlineData("[[1,2,-1,-5],[2,-1,2,8],[3,3,4,5]]", new double[] { 1, -2, 2 })]
    [InlineData("[[2,1,-1,8],[-3,-1,2,-11],[-2,1,2,-3]]", new double[] { 2, 3, -1 })]
    [InlineData("[[1,1,0,0,0,144],[0,1,1,0,0,109],[0,0,1,1,0,85],[0,0,0,1,1,70],[1,1,1,1,1,260]]", new double[] { 81, 63, 46, 39, 31 })]
    [InlineData("[[1,1,1,1,1,260],[1,1,0,0,0,144],[0,1,1,0,0,109],[0,0,1,1,0,85],[0,0,0,1,1,70]]", new double[0])]
    [InlineData("[[0,1,1,0,0,109],[0,0,1,1,0,85],[0,0,0,1,1,70],[1,1,1,1,1,260],[1,1,0,0,0,144]]", new double[0])]
    public void SolveLinearEquations(string matrixJson, double[] expected)
        {
        double[][] matrix = JsonSerializer.Deserialize<double[][]>(matrixJson) ?? throw new ArgumentException("Invalid matrix format");

        double[] solution = Maths.SolveLinearEquations(matrix);

        output.WriteLine(JsonSerializer.Serialize(matrix));
        output.WriteLine(JsonSerializer.Serialize(solution));

        Assert.Equal(expected, solution);
        //Assert.True(Maths.AreEqual(expected, solution, 0.0001));
        }

    /// <summary>
    /// Problema de Monty Hall
    /// </summary>
    /// <see cref="//https://es.wikipedia.org/wiki/Problema_de_Monty_Hall"/>
    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void MontyHall(int n)
        {
        int successCount = 0;
        for (int i = 0; i < n; i++)
            {
            int iPrize = RandomWrapper.Shared.Next(3);
            int iSelect = RandomWrapper.Shared.Next(3);

            //if (iPrize != iSelect) { successCount++; }

            if (iPrize == 0 && iSelect == 0) { iSelect = 1; }   // show 2 (same as 1)
            if (iPrize == 0 && iSelect == 1) { iSelect = 0; }   // show 2
            if (iPrize == 0 && iSelect == 2) { iSelect = 0; }   // show 1
            if (iPrize == 1 && iSelect == 0) { iSelect = 1; }   // show 2
            if (iPrize == 1 && iSelect == 1) { iSelect = 2; }   // show 0 (same as 2)
            if (iPrize == 1 && iSelect == 2) { iSelect = 1; }   // show 0
            if (iPrize == 2 && iSelect == 0) { iSelect = 2; }   // show 1
            if (iPrize == 2 && iSelect == 1) { iSelect = 2; }   // show 0
            if (iPrize == 2 && iSelect == 2) { iSelect = 1; }   // show 0 (same as 1)

            if (iPrize == iSelect) { successCount++; }
            }

        double successRatio = (double)successCount / n;
        output.WriteLine(successRatio.ToString());
        Assert.Equal(2.0/3, successRatio, 1.0 / (n / 100));
        }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void MontyHallWithoutChange(int n)
        {
        int successCount = 0;
        for (int i = 0; i < n; i++)
            {
            int iPrize = RandomWrapper.Shared.Next(3);
            int iSelect = RandomWrapper.Shared.Next(3);

            if (iPrize == iSelect) { successCount++; }
            }

        double successRatio = (double)successCount / n;
        output.WriteLine(successRatio.ToString());
        Assert.Equal(1.0/3, successRatio, 1.0 / (n / 100));
        }

    /// <summary>
    /// Como medir 1 m con una vara de 0.6 y otra de 0.7
    /// </summary>
    [Fact]
    public void Varas()
        {
        for (int i = 0; i < 10; i++)
            {
            for (int j = 0; j < 10; j++)
                {
                if (i * 0.6 - j * 0.7 == 1)
                    {
                    output.WriteLine($"{i} * 0.6 - {j} * 0.7 = 1");
                    return;
                    }
                if (j * 0.6 - i * 0.7 == 1)
                    {
                    output.WriteLine($"{j} * 0.6 - {i} * 0.7 = 1");
                    return;
                    }
                }
            }

        output.WriteLine($"Sin solución.");
        }
    }
