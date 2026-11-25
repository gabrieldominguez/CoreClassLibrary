#pragma warning disable IDE0046 // Convert to conditional expression

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Ranges;
using System.Buffers;
using System.Numerics;

namespace CoreClassLibrary.Mathematics;

/// <summary>
/// Provides constants and static methods for common mathematical functions.
/// </summary>
/// <remarks>
/// ENTEROS:
/// Cuando un literal entero no tiene sufijo, su tipo es el primero de estos tipos en el que se puede representar su valor: int, uint, long, ulong.
/// Cuando se utiliza el sufijo L, el tipo del entero literal será long o ulong, según su tamaño.
/// Cuando se utiliza el sufijo U o u, el tipo del literal puede ser uint o ulong según su tamaño.
///
/// REALES:
/// Si no se especifica el sufijo de tipo real, el tipo del literal real es double. De lo contrario, el sufijo de tipo real determina el tipo del literal real, como sigue:
///
/// Un literal real con el sufijo F o f es de tipo float. Por ejemplo, los literales 1f, 1.5f, 1e10f y 123.456F son de tipo float.
/// Un literal real con el sufijo D o d es de tipo double. Por ejemplo, los literales 1d, 1.5d, 1e10d y 123.456D son de tipo double.
/// Un literal real con el sufijo M o m es de tipo decimal. Por ejemplo, los literales 1m, 1.5m, 1e10m y 123.456M son de tipo decimal.
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/generics/math"/>
/// <see cref="https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/"/>
public static class Maths
    {
    /// <see cref="https://en.wikipedia.org/wiki/Golden_ratio"/>
    /// <see cref="https://www.mathsisfun.com/numbers/golden-ratio.html"/>
    public static readonly double Phi = 0.5 + Math.Sqrt(5.0) / 2.0;  // Golden Ratio:

    /// <see cref="https://es.wikipedia.org/wiki/Rect%C3%A1ngulo_cordob%C3%A9s"/>
    public static readonly double Cordoba = Math.Sqrt((1.0 + Math.Sqrt(2.0)) / Math.Sqrt(2.0));  // Cordoba Ratio:

    /// <summary>
    /// Newton's method for integer sqrt
    /// </summary>
    public static TSelf IntegerSqrt<TSelf>(this TSelf value) where TSelf : IBinaryInteger<TSelf>
        {
        TSelf two = TSelf.CreateChecked(2);
        if (value < two) 
            { 
            return value;
            };

        TSelf x = value / two;
        TSelf lastX;

        do
            {
            lastX = x;
            x = (x + value / x) / two;
            }
        while (x < lastX);

        return lastX;
        }


    public static int? BinarySearchOrDefault<T>(this IReadOnlyList<T> sortedList, T target)
        where T : INumber<T>
        {
        int index = Maths.BinarySearch<T>(sortedList, target);
        return sortedList[index] == target? index: default;
        }
    
    /// <summary>
    /// Binary Search: Performs a binary search on a sorted <see cref="IReadOnlyList{T}"/> to find the index of the specified target value.
    /// </summary>
    /// <typeparam name="T">The element type of the list.</typeparam>
    /// <param name="sortedList">The sorted list to search. It must be sorted in ascending order.</param>
    /// <param name="target">The value to search for.</param>
    /// <returns>
    /// The zero-based index of the <paramref name="target"/> in the list if found; 
    /// otherwise, the bitwise complement (~) of the index of the next element that is larger than <paramref name="target"/>.
    /// </returns>
    /// <remarks>
    /// This method assumes that <paramref name="orderedList"/> is sorted in ascending order.
    /// If it is not sorted, the result is undefined.
    /// 
    /// The return behavior matches that of <see cref="Array.BinarySearch(Array, object)"/>:
    /// - If the target is found, returns its index (>= 0).
    /// - If the target is not found, returns the bitwise complement (~) of the insertion point.
    /// 
    /// The insertion point is the index of the first element that is greater than the target.
    /// </remarks>
    /// <see cref="Maths.DichotomicOptimization"/>
    /// <see cref="Array.BinarySearch"/>
    /// <see cref="https://en.wikipedia.org/wiki/Binary_search"/>
    public static int BinarySearch<T>(this IReadOnlyList<T> sortedList, T target)
        where T : IComparable<T>
        //=> Maths.DichotomicOptimization<int, T>(0, sortedList.Count - 1, target, i => sortedList[i], 0, T.Zero);  //
        {
        int minX = 0;
        int maxX = sortedList.Count - 1;

        while (minX <= maxX)
            {
            int i = (minX + maxX) / 2;
            int cmp = sortedList[i].CompareTo(target);

            if (cmp == 0)
                {
                return i;
                }
            
            if (cmp < 0)
                {
                minX = i + 1;
                }
            else
                {
                maxX = i - 1;
                }
            }

        // not found -> bitwise complement of insertion index
        return ~minX;
        }

    public static Tx DichotomicOptimization<Tx, Ty>(Tx minX, Tx maxX, Ty targetY, Func<Tx, Ty> f, Ty precisionY)
        where Tx : INumber<Tx>
        where Ty : INumber<Ty>
        => Maths.DichotomicOptimization<Tx, Ty>(minX, maxX, targetY, f, Tx.Zero, precisionY);

    /// <summary>
    /// Permite optimizar el parametro de una funcion lineal creciente, obteniendo el valor de x para un valor de y especificado.
    /// </summary>
    /// <param name="minX">Valor mínimo de x.</param>
    /// <param name="maxX">Valor máximo de x.</param>
    /// <param name="targetY">Valor de y retornado por la función cuyo x se quiere conocer.</param>
    /// <param name="f">Función lineal creciente a optimizar.</param>
    /// <param name="precisionY">Diferencia entre el valor de y calculado y de target_y para los que se consideran iguales.</param>
    /// <param name="precisionX">Rango minimo en X hasta donde buscar.</param>
    /// <returns>Valor de x para el cual la función retorna target_y.</returns>
    /// <remarks>
    /// La funcion f a optimizar debe ser lineal creciente y continua dentro del intervalo [min_x, max_x] y
    /// target_y debe pertenecer al intervalo [f(min_x), f(max_x)].
    /// </remarks>
    public static Tx DichotomicOptimization<Tx, Ty>(Tx minX, Tx maxX, Ty targetY, Func<Tx, Ty> f, Tx precisionX, Ty precisionY) 
        where Tx : INumber<Tx>
        where Ty : INumber<Ty>
        {
        Tx x;
        Ty y;

        Ty minY = f(minX);
        Ty maxY = f(maxX);

        new Range<Ty>(minY, maxY).ThrowIfNotContains(targetY);

        Tx two = Tx.CreateChecked(2);
        do
            {
            if ((y = f(x = (minX + maxX) / two)) > targetY)
                {
                maxX = x;
                }
            else
                {
                minX = x;
                }
            }
        while (!Maths.AreEqual(y, targetY, precisionY) && !Maths.AreEqual(minX, maxX, precisionX));

        return x;
        }

    public static bool IsEven<TSelf>(this TSelf value) where TSelf: INumber<TSelf>
        => TSelf.IsEvenInteger(value);
        //=> value % TSelf.CreateChecked(2) == TSelf.Zero;

    /// <summary>
    /// Compara dos valores, teniendo en cuenta un intervalo dentro del que se consideran iguales.
    /// </summary>
    /// <returns>True si los valores se consideran iguales.</returns>
    public static bool AreEqual<TSelf>(TSelf val1, TSelf val2, TSelf precision) where TSelf: INumber<TSelf>
        => TSelf.Abs(val1 - val2) <= precision;

    public static bool AreEqual<TSelf>(IEnumerable<TSelf> source1, IEnumerable<TSelf> source2, TSelf precision) where TSelf: INumber<TSelf>
        => EnumerableExtensions.EnumerateInTuples(source1, source2).All(tuple => Maths.AreEqual(tuple.Item1, tuple.Item2, precision));

    /// <summary>
    /// Calcula el producto de todos los numeros de la secuencia.
    /// </summary>
    /// <remarks>Si la secuencia está vacia retorna 0.</remarks>
    /// <returns>El producto de todos los numeros de la secuencia o 0 si está vacia.</returns>
    public static TSource Prod<TSource>(this IEnumerable<TSource> source) 
        where TSource : INumber<TSource>
        => source.Prod<TSource, TSource>();

    public static TResult Prod<TSource, TResult>(this IEnumerable<TSource> source)
        where TSource : INumber<TSource>
        where TResult : INumber<TResult>
        => source.Aggregate(TResult.MultiplicativeIdentity, (prod, x) => prod * TResult.CreateChecked(x));   

    /// <summary>
    /// Computes the sum of the sequence of values.
    /// </summary>
    public static TSource Sum<TSource>(this IEnumerable<TSource> source) 
        where TSource : INumber<TSource>
        => source.Sum<TSource, TSource>();

    public static TResult Sum<TSource, TResult>(this IEnumerable<TSource> source) 
        where TSource : INumber<TSource>
        where TResult : INumber<TResult>
        => source.Aggregate(TResult.AdditiveIdentity, (sum, x) => sum + TResult.CreateChecked(x));

    /// <summary>
    /// Permite aplicar una funcion a todos los items de la secuencia de a pares y devolver la suma de todos los resultados.
    /// </summary>
    /// <returns>La suma de todos los resultados de la funcion aplicada a todos los pares de items continuos.</returns>
    /// Tambien puede implementarse con AggregateNext()
    public static TSelf SumPairs<TSelf>(this IEnumerable<TSelf> source, Func<TSelf?, TSelf, TSelf> func) where TSelf: IAdditionOperators<TSelf, TSelf, TSelf>, IAdditiveIdentity<TSelf, TSelf>
        => source.AggregatePrevious<TSelf, TSelf>((previous, current, accumulate, isFirst) => isFirst ? TSelf.AdditiveIdentity : accumulate + func(previous, current), TSelf.AdditiveIdentity);

    private static readonly long[] factors64 = [1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600, 6227020800, 87178291200, 1307674368000, 20922789888000, 355687428096000, 6402373705728000, 121645100408832000, 2432902008176640000];
    /// <summary>
    /// f(x) = x!
    /// </summary>
    /// <returns>x!</returns>
    /// <see cref="http://www.guyellisrocks.com/2010/07/factorial-function-in-c.html"/>
    //public static long Fact(this int x) => x <= 1? 1: x * MathHelper.Fact(x-1);
    //public static long Fact(this int x) => x <= 1? 1: Enumerable.Range(2, x-1).Select(n => (long)n).Prod();
    public static long Fact(this int x)
        => x < Maths.factors64.Length? Maths.factors64[x]: throw new InvalidOperationException(String.Format("x ({0}) must be less than {1}.", x, Maths.factors64.Length));

    /// <summary>
    /// Idem Math.Pow pero con parametros decimal.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static decimal Pow(decimal x, decimal y)
        => (decimal)Maths.Pow<double>((double)x, (double)y);

    public static TSelf Pow2<TSelf>(this TSelf x) 
        where TSelf : IMultiplyOperators<TSelf, TSelf, TSelf> 
        => Maths.Pow2<TSelf, TSelf>(x);

    public static TResult Pow2<TSelf, TResult>(this TSelf x) 
        where TSelf : IMultiplyOperators<TSelf, TSelf, TResult> 
        => x * x;

    public static TResult Pow3<TSelf, TResult>(this TSelf x) 
        where TSelf : IMultiplyOperators<TSelf, TSelf, TResult> 
        where TResult : IMultiplyOperators<TResult, TSelf, TResult>  
        => x * x * x;

    public static TResult Pow4<TSelf, TResult>(this TSelf x) 
        where TSelf : IMultiplyOperators<TSelf, TSelf, TResult> 
        where TResult : IMultiplyOperators<TResult, TSelf, TResult>  
        => x * x * x * x;

    public static TResult Pow5<TSelf, TResult>(this TSelf x) 
        where TSelf : IMultiplyOperators<TSelf, TSelf, TResult> 
        where TResult : IMultiplyOperators<TResult, TSelf, TResult>  
        => x * x * x * x * x;

    public static TSelf Pow<TSelf>(this TSelf x, int y) 
        where TSelf : INumber<TSelf>, IPowerFunctions<TSelf>
        => y switch
            {
            0 => TSelf.One, //.MultiplicativeIdentity,
            1 => x,
            2 => x.Pow2<TSelf, TSelf>(),
            3 => x.Pow3<TSelf, TSelf>(),
            4 => x.Pow4<TSelf, TSelf>(),
            5 => x.Pow5<TSelf, TSelf>(),
            _ => TSelf.Pow(x, TSelf.CreateChecked(y))
            };

    public static TSelf Pow<TSelf>(this TSelf x, TSelf y) 
        where TSelf : IPowerFunctions<TSelf>
        => y switch
            {
            0 => TSelf.One, //.MultiplicativeIdentity,
            1 => x,
            2 => x.Pow2<TSelf, TSelf>(),
            3 => x.Pow3<TSelf, TSelf>(),
            4 => x.Pow4<TSelf, TSelf>(),
            5 => x.Pow5<TSelf, TSelf>(),
            _ => TSelf.Pow(x, y)
            };

    /// <summary>
    /// Generic version of Math.Pow(10, y)
    /// </summary>
    public static TSelf TenPow<TSelf>(int y) where TSelf : INumber<TSelf>
        {   
        TSelf result = TSelf.MultiplicativeIdentity;
        TSelf ten = TSelf.CreateChecked(10);

        if (y > 0)
            {
            for (int i = 0; i < y; i++)
                {
                result *= ten;
                }
            }

        if (y < 0)
            {
            for (int i = 0; i > y; i--)
                {
                result /= ten;
                }
            }
            
        return result;
        }

    /// <summary>
    /// Redondea un valor a un intervalo determinado.
    /// </summary>
    /// <param name="value">Valor a redondear.</param>
    /// <param name="scale">Valor que determina el intervalo.</param>
    /// <returns>Valor a redondear.</returns>
    public static TSelf RoundRange<TSelf>(this TSelf value, TSelf scale, MidpointRounding mode = MidpointRounding.ToEven) where TSelf : IFloatingPoint<TSelf>
        => TSelf.Round(value / scale, mode) * scale;

    /// <summary>
    /// Trunca un valor a un intervalo determinado.
    /// </summary>
    /// <param name="d">Valor a redondear.</param>
    /// <param name="c">Valor que determina el intervalo.</param>
    /// <returns>Valor a truncar.</returns>
    public static TSelf TruncateRange<TSelf>(this TSelf value, TSelf scale) where TSelf : IFloatingPoint<TSelf>
        => TSelf.Truncate(value / scale) * scale;

    /// <summary>
    /// Truncate a decimal value to the specified precision.
    /// </summary>
    /// <param name="value">A decimal number to be truncated.</param>
    /// <param name="digits">The number of fractional digits (precision) in the return value. </param>
    /// <returns>The smaller number with a precision equal to digits.</returns>
    public static TSelf Truncate<TSelf>(this TSelf value, int digits = 0) where TSelf : IFloatingPoint<TSelf>
        => TSelf.Round(value, digits, MidpointRounding.ToZero);

    /// <summary>
    /// Trims the leading zero from bytes enumerable.
    /// </summary>
    public static IEnumerable<TSelf> TrimLeadingZero<TSelf>(this IEnumerable<TSelf> buffer)
        where TSelf : INumber<TSelf>
        {
        bool leadingZero = true;
        foreach (TSelf item in buffer)
            {
            if (item == TSelf.Zero && leadingZero)
                {
                continue;
                }
            else
                {
                leadingZero = false;
                }

            yield return item;
            }
        }

    /// <summary>
    /// Converts an array of 16 bytes guid to its equivalent string representation that is encoded with base-64 digits.
    /// </summary>
    /// <returns>24 character string Guid</returns>
    /// <see cref="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c2a9857d-ecdc-489d-a534-84569d2e8e02/generate-guid-20-or-30-character?forum=netfxbcl"/>
    public static string ToBase64String(this Guid guid)
        => Convert.ToBase64String(guid.ToByteArray());

    /// <summary>
    /// Determina si un vector de n dimensiones está contenido dentro de un espacio definido por rangos de cada dimension.
    /// </summary>
    public static bool Contains<T>(IEnumerable<Range<T>> space, IEnumerable<T> vector) where T : IComparable<T>
        => EnumerableExtensions
                .EnumerateInTuples(space, vector)
                .All(tuple => tuple.Item1.Contains(tuple.Item2));

    /// <summary>
    /// Convierte un espacio definido por tuplas de cada n dimension, a una tupla de vectores de n dimensiones.
    /// </summary>
    /// <remarks>
    /// espacio     =>      tupla de vectores
    ///
    /// (x1 - x2) |
    /// (y1 - y2) |=> (x1, y1, z1) - (x2, y2, z2)
    /// (z1 - z2) |
    ///
    /// </remarks>
    public static (T[], T[]) SpaceToVectorTuple<T>((T, T)[] space)
        => (
           space.Select(tuple => tuple.Item1).ToArray(space.Length),
           space.Select(tuple => tuple.Item2).ToArray(space.Length)
           );

    public static int GetHashCode(object key1, object key2)
        => key1.GetHashCode() ^ key2.GetHashCode();

    public static TSelf GetHashCode<TSelf>(TSelf key1, TSelf key2) where TSelf : IBinaryNumber<TSelf> 
        => key1 ^ key2;

    public static ref TSelf RefMin<TSelf>(ref TSelf left, ref TSelf right) where TSelf : IComparisonOperators<TSelf, TSelf, bool> 
        {
        if (left < right)
            {
            return ref left;
            }
        else
            {
            return ref right;
            }
        }

    public static ref TSelf RefMax<TSelf>(ref TSelf left, ref TSelf right) where TSelf : IComparisonOperators<TSelf, TSelf, bool> 
        {
        if (left > right)
            {
            return ref left;
            }
        else
            {
            return ref right;
            }
        }

    public static T Min<T>(T x, T y, T z, T w) where T : INumber<T> 
        => T.Min(T.Min(T.Min(x, y), z), w);
    public static T Min<T>(T x, T y, T z) where T : INumber<T> 
        => T.Min(T.Min(x, y), z);
    public static T Min<T>(T x, T y) where T : INumber<T> 
        => T.Min(x, y);

    public static T Max<T>(T x, T y, T z, T w) where T : INumber<T> 
        => T.Max(T.Max(T.Max(x, y), z), w);
    public static T Max<T>(T x, T y, T z) where T : INumber<T> 
        => T.Max(T.Max(x, y), z);
    public static T Max<T>(T x, T y) where T : INumber<T> 
        => T.Max(x, y);

    /// <summary>
    /// Get EAN-13 / EAN-8 Check Digit
    /// </summary>
    /// <remarks>
    /// EAN-13 Check Digit is used for validating the integrity of EAN-13 barcodes, which are widely used for retail products.
    /// Also is used for validating the integrity of ISBN-13 codes, which are used for books. 
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/International_Article_Number#Check_digit"/>
    /// <see cref="https://fj-techtools.com/guides/ean13-check-digit"/>
    /// <seealso cref="https://isbndecoder.com/how-isbn-check-digit-works"/>
    /// <seealso cref="https://barcodepress.com/guides/ean-13-check-digit-calculation"/>
    public static char GetEANCheckDigit(this string number)
        => number.GetCheckDigit([3, 1], 10);

    /// <summary>
    /// Algoritmo modulo de suma ponderada para calcular el digito verificador de un numero.
    /// </summary>
    /// <param name="number">Cadena que contiene un numero.</param>
    /// <param name="weights">
    /// Ponderadores a utilizar en la suma, son aplicados de derecha a izquierda.
    /// Puede ser una secuencia de numeros consecutivos o impares o primos.
    /// CBU usa { 3, 1, 7, 9 }
    /// CUIT usa { 2, 3, 4, 5, 6, 7 }
    /// CUENTA RS usa { 2, 3, 4, 5, 6, 9, 8, 7, 8, 9 }
    /// </param>
    /// <param name="modulo">
    /// Modulo a aplicar a la suma para obtener el digito verificador.
    /// Los modulos mas comunes son 10 y 11.
    /// Si se utiliza modulo 11, cuando el resultado sea 10 retornará ':', que es un caracter no numerico.
    /// EAN13 usa modulo 10
    /// ISBN usa EAN13
    /// CBU usa modulo 10
    /// CUIT usa modulo 11
    /// CUENTA RS usa modulo 11
    /// </param>
    /// <returns>El caracter correspondiente al digito verificador calculado.</returns>
    /// <see cref="https://en.wikipedia.org/wiki/Checksum"/>
    /// <see cref="https://en.wikipedia.org/wiki/Check_digit"/>
    /// <see cref="https://es.wikipedia.org/wiki/C%C3%B3digo_de_control"/>
    /// <see cref="https://en.wikipedia.org/wiki/Weight_function"/>
    /// <seealso cref="https://es.wikipedia.org/wiki/Clave_Bancaria_Uniforme#Algoritmo_de_validaci%C3%B3n"/>
    /// <seealso cref="https://es.wikipedia.org/wiki/Clave_%C3%9Anica_de_Identificaci%C3%B3n_Tributaria#Procedimiento_para_obtener_el_d%C3%ADgito_verificador"/>
    public static char GetCheckDigit(this string number, IReadOnlyList<int> weights, int modulo = 10)
        {
        int sum = 0;
        for (int i = 0; i < number.Length; i++)
            {
            sum += (number[i] - '0') * weights[(number.Length - i - 1) % weights.Count];
            }

        int remainder = sum % modulo;
        return (char)('0' + (remainder == 0 ? 0 : modulo - remainder));
        //return (char)('0' + ((modulo - sum % modulo) % modulo));
        }
    
    // https://en.wikipedia.org/wiki/Three-valued_logic
    // null => unknown
    // public static bool? Not(this bool? a) => !a;

    public static bool? And(this bool? a, bool? b)
        => a == false || b == false ? false : 
            a == null || b == null ? null : true;

    public static bool? Or(this bool? a, bool? b)
        => a == true || b == true ? true : 
            a == null || b == null ? null : false;

    public static bool? Xor(this bool? a, bool? b)
        => a == null || b == null ? null : a.Value ^ b.Value;

    /// <summary>
    /// Cantor pairing function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Pairing_function"/>
    public static TSelf CantorPairing<TSelf>(TSelf x, TSelf y) where TSelf: INumber<TSelf>
        => (x + y) * (x + y + TSelf.One) / TSelf.CreateChecked(2) + y;

    /// <summary>
    /// Cantor pairing inverse function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Pairing_function"/>
    public static (TSelf x, TSelf y) CantorPairingInverse<TSelf>(TSelf z) where TSelf: INumber<TSelf>, IRootFunctions<TSelf>, IFloatingPoint<TSelf>
        {
        TSelf two = TSelf.CreateChecked(2);

        TSelf w = TSelf.Floor((TSelf.Sqrt(TSelf.CreateChecked(8) * z + TSelf.One) - TSelf.One) / two);
        TSelf y = z - (w.Pow2() + w) / two;
        return (w - y, y);
        }
    
    /// <summary>
    /// Gauss-Jordan Matrix Elimination
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Gaussian_elimination"/>
    /// <see cref="https://brilliant.org/wiki/gaussian-elimination/"/>
    /// <see cref="https://rephip.unr.edu.ar/items/1e60d81c-de65-4a68-8aa6-eb4150be70bd"/>
    /// <seealso cref="https://www.emathhelp.net/calculators/linear-algebra/gauss-jordan-elimination-calculator"/>
    /// <seealso cref="https://codereview.stackexchange.com/questions/79462/better-implementation-of-gaussian-elimination"/>
    public static void ComputeGaussianElimination<TSelf>(TSelf[][] matrix) where TSelf: INumber<TSelf>
        {
        int columnsLength = matrix[0].Length;
        int rowsLength = matrix.Length;

        for (int r1 = 0, c1 = 0; r1 < rowsLength; r1++, c1++)
            {
            for (int r2 = r1+1; r2 < rowsLength; r2++)
                {
                for (int c2 = c1+1; c2 < columnsLength; c2++)
                    {
                    matrix[r2][c2] = matrix[r1][c1] * matrix[r2][c2] - matrix[r2][c1] * matrix[r1][c2];
                    }
                matrix[r2][c1] = TSelf.Zero;
                }               
            }
        }

    public static TSelf[] SolveGaussianElimination<TSelf>(TSelf[][] matrix) where TSelf: INumber<TSelf>
        {
        int columnsLength = matrix[0].Length;
        int rowsLength = matrix.Length;

        TSelf[] solution = new TSelf[rowsLength];

        for (int r = rowsLength-1, c1 = columnsLength-2; r >= 0; r--, c1--)
            {
            TSelf rowTotal = matrix[r][columnsLength-1];

            for (int c2 = columnsLength-2; c2 > c1; c2--)
                {
                rowTotal -= matrix[r][c2] * solution[c2];
                }

            solution[r] = rowTotal / matrix[r][c1];

            if (TSelf.IsNaN(solution[r]) || TSelf.IsInfinity(solution[r]))
                {
                return [];
                }
            }

        return solution;
        }

    public static TSelf[] SolveLinearEquations<TSelf>(TSelf[][] matrix) where TSelf: INumber<TSelf>
        {
        Maths.ComputeGaussianElimination(matrix);
        return Maths.SolveGaussianElimination(matrix);
        }
    }
