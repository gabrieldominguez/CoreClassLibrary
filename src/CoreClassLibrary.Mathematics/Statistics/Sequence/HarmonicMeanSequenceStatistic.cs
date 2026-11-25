using System.Numerics;

namespace CoreClassLibrary.Mathematics.Statistics.Sequence;

/// <summary>
/// Calcula la media harmonica de los valores del arreglo.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Harmonic_mean"/>
/// <see cref="https://en.wikipedia.org/wiki/Mean"/>
public class HarmonicMeanSequenceStatistic<T>() : GeneralizedMeanSequenceStatistic<T>(-1) where T : INumber<T>, IPowerFunctions<T>;
