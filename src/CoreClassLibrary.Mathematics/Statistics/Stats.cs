#pragma warning disable IDE0046 // Convert to conditional expression

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics;

public static class Stats
    {
    /// <summary>
    /// Rango medio: (min + max) / 2
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Mid-range"/>
    public static TSource MidRange<TSource>(this IEnumerable<TSource> values)
        where TSource : INumber<TSource>
        {
        (TSource min, TSource max) = values.MinMax();
        return (min + max) / TSource.CreateChecked(2);
        }

    public static TSource Median<TSource>(this IReadOnlyList<TSource> values)
        => values.Quantile(2);

    public static TSource Quartile<TSource>(this IReadOnlyList<TSource> values)
        => values.Quantile(4);

    /// <summary>
    /// Retorna el elemento del quantile especificado.
    /// </summary>
    /// <remarks>El arreglo debe estar ordenado.</remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile"/>
    /// <see cref="https://en.wikipedia.org/wiki/Median"/>
    /// <see cref="https://en.wikipedia.org/wiki/Mean"/>
    public static TSource Quantile<TSource>(this IReadOnlyList<TSource> values, int q)
        => values[values.Count / q];

    public static (TSource Mean, TSource StandardDeviation) ComputeMeanAndStandardDeviation<TSource>(this IEnumerable<TSource> values) 
        where TSource : INumber<TSource>, IRootFunctions<TSource>
        => values.ComputeMeanAndStandardDeviation<TSource, int>();

    /// <summary>
    /// Calcula el promedio y la desviacion estandar.
    /// </summary>
    /// <returns>
    /// Una tupla con el promedio y la desviacion estandar, respectivamente.
    /// Acepta un arreglo en lugar de una enumeracion porque se necesita enumerarlo dos veces.
    /// </returns>
    /// <see cref="https://en.wikipedia.org/wiki/Standard_deviation"/>
    public static (TSource Mean, TSource StandardDeviation) ComputeMeanAndStandardDeviation<TSource, TCount>(this IEnumerable<TSource> values) 
        where TSource : INumber<TSource>, IRootFunctions<TSource>  
        where TCount : INumber<TCount>
        {
        (TSource mean, TSource variance) = values.ComputeMeanAndVariance<TSource, TCount>();
        return (mean, TSource.Sqrt(variance));
        }

    public static (TSource Mean, TSource Variance) ComputeMeanAndVariance<TSource>(this IEnumerable<TSource> values) 
        where TSource : INumber<TSource>
        => values.ComputeMeanAndVariance<TSource, int>();

    /// <summary>
    /// Calcula el promedio y la desviacion estandar.
    /// </summary>
    /// <returns>
    /// Una tupla con el promedio y la varianza, respectivamente.
    /// Acepta un arreglo en lugar de una enumeracion porque se necesita enumerarlo dos veces.
    /// </returns>
    /// <see cref="https://en.wikipedia.org/wiki/Standard_deviation"/>
    public static (TSource Mean, TSource Variance) ComputeMeanAndVariance<TSource, TCount>(this IEnumerable<TSource> values) 
        where TSource : INumber<TSource>
        where TCount : INumber<TCount>
        {
        TSource mean = values.Mean<TSource, TCount>();        
        return (mean, values.Select(val => (val - mean).Pow2()).Mean<TSource, TCount>());
        }

    public static TSource Mean<TSource>(this IEnumerable<TSource> source) 
        where TSource : INumber<TSource>
        => source.Mean<TSource, int>();

    /// <summary>
    /// Calcula la media aritmetica de los items de la secuencia.
    /// </summary>
    /// <remarks>
    /// TSource.CreateSaturating(count): 
    /// Saturation is the default behavior for IEEE 754 floating-point types as 
    /// they can represent positive and negative infinity as their respective minimum and maximum values.
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/Arithmetic_mean"/>
    public static TSource Mean<TSource, TCount>(this IEnumerable<TSource> source)
        where TSource : INumber<TSource>
        where TCount : INumber<TCount>
        {
        (TCount count, TSource sum) = source.CountAndSum<TSource, TCount>();

        if (count == TCount.Zero)
            {
            throw new InvalidOperationException("Sequence contains no elements");
            }

        return TSource.CreateChecked(sum) / TSource.CreateSaturating(count);
        }

    public static (int Count, TSource Sum) CountAndSum<TSource>(this IEnumerable<TSource> source) 
        where TSource : IAdditionOperators<TSource, TSource, TSource>, IAdditiveIdentity<TSource, TSource>
        => source.CountAndSum<TSource, int>();

    /// <summary>
    /// Cuenta y suma los items de la secuencia.
    /// </summary>
    /// <remarks>
    /// La enumeración es enumerada una única vez.
    /// </remarks>
    /// <returns>
    /// Una tupla con la cantidad y la suma, respectivamente.
    /// </returns>
    public static (TCount Count, TSource Sum) CountAndSum<TSource, TCount>(this IEnumerable<TSource> source) 
        where TSource : IAdditionOperators<TSource, TSource, TSource>, IAdditiveIdentity<TSource, TSource>
        where TCount : IIncrementOperators<TCount>, IAdditiveIdentity<TCount, TCount>
        {
        TCount count = TCount.AdditiveIdentity;
        TSource sum = TSource.AdditiveIdentity;

        foreach (TSource value in source)
            {
            count++;
            sum += value;
            }

        return (count, sum);
        }

    public static (int Count, TSource Sum) CountAndProd<TSource>(this IEnumerable<TSource> source) 
        where TSource : IMultiplyOperators<TSource, TSource, TSource>, IMultiplicativeIdentity<TSource, TSource>
        => source.CountAndProd<TSource, int>();

    /// <summary>
    /// Cuenta y calcula el producto de los items de la secuencia.
    /// </summary>
    /// <remarks>
    /// La enumeración es enumerada una única vez.
    /// </remarks>
    /// <returns>
    /// Una tupla con la cantidad y el producto, respectivamente.
    /// </returns>
    public static (TCount Count, TSource Prod) CountAndProd<TSource, TCount>(this IEnumerable<TSource> source) 
        where TSource : IMultiplyOperators<TSource, TSource, TSource>, IMultiplicativeIdentity<TSource, TSource>
        where TCount : IIncrementOperators<TCount>, IAdditiveIdentity<TCount, TCount>
        {
        TCount count = TCount.AdditiveIdentity;
        TSource prod = TSource.MultiplicativeIdentity;

        foreach (TSource value in source)
            {
            count++;
            prod *= value;
            }

        return (count, prod);
        }

    public static IEnumerable<TSource> MovingMean<TSource>(this IEnumerable<TSource> values)
        where TSource : INumber<TSource>
        => values.MovingMean<TSource, int>();

    /// <summary>
    /// Calcula el promedio de los valores enumerados hasta el momento.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Moving_mean"/>
    public static IEnumerable<TSource> MovingMean<TSource, TCount>(this IEnumerable<TSource> values)
        where TSource : INumber<TSource>
        where TCount : INumber<TCount>
        {
        TCount count = TCount.AdditiveIdentity;
        TSource sum = TSource.AdditiveIdentity;

        foreach (TSource value in values)
            {
            yield return TSource.CreateChecked(sum += value) / TSource.CreateChecked(++count);
            }
        }

    public static IEnumerable<TSource> ComputeGaussNormalization<TSource>(this IEnumerable<TSource> values) 
        where TSource : INumber<TSource>, IRootFunctions<TSource>
        => values.ComputeGaussNormalization<TSource, int>();

    /// <summary>
    /// Compute z-score Gauss normalization
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Standard_score"/>
    public static IEnumerable<TSource> ComputeGaussNormalization<TSource, TCount>(this IEnumerable<TSource> values) 
        where TSource : INumber<TSource>, IRootFunctions<TSource>
        where TCount : INumber<TCount>
        {
        (TSource mean, TSource standardDeviation) = values.ComputeMeanAndStandardDeviation<TSource, TCount>();

        if (standardDeviation == TSource.Zero) 
            {
            throw new ArithmeticException("All values are the same.");
            }

        return values.Select(val => (val - mean) / standardDeviation);
        }

    public static IEnumerable<TSource> ComputeMinMaxNormalization<TSource>(this IEnumerable<TSource> values)
        where TSource : INumber<TSource>
        {
        (TSource min, TSource max) = values.MinMax();

        TSource range = max - min;

        if (range == TSource.Zero)
            {
            throw new ArithmeticException("All values are the same.");
            }

        return values.Select(val => (val - min) / range);
        }

    public static TSource ComputeErrorSum<TSource, TCount, TSum>(IEnumerable<TSource> targetValues, IEnumerable<TSource> values)
        where TSource : INumber<TSource>
        where TCount : INumber<TCount>
        =>  EnumerableExtensions
                .EnumerateInTuples(targetValues, values)
                .Select(tuple => TSource.Abs(tuple.Item1 - tuple.Item2))
                .Mean<TSource, TCount>();

    public static TSource ComputeMeanSquaredErrorSum<TSource, TCount, TSum>(IEnumerable<TSource> targetValues, IEnumerable<TSource> values)
        where TSource : INumber<TSource>
        where TCount : INumber<TCount>
        =>  EnumerableExtensions
                .EnumerateInTuples(targetValues, values)
                .Select(tuple => TSource.Abs(tuple.Item1 - tuple.Item2))
                .Select(Maths.Pow2<TSource, TSource>)
                .Mean<TSource, TCount>();

    public static TSource MeanCrossEntropyErrorSum<TSource, TCount, TSum>(IEnumerable<TSource> targetValues, IEnumerable<TSource> values)
        where TSource : INumber<TSource>, ILogarithmicFunctions<TSource>
        where TCount : INumber<TCount>
        =>  EnumerableExtensions
                .EnumerateInTuples(targetValues, values)
                .Select(tuple => TSource.Log(tuple.Item2) * tuple.Item1)
                .Mean<TSource, TCount>();
    
    }
