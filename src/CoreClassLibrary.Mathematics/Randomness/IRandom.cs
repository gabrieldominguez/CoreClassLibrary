
namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Interface que deben implementar los generadores de muestras.
/// </summary>
public interface IRandom
    {
    /// <summary>
    /// Generate a random number.
    /// </summary>
    /// <returns>A double [0..1)</returns>
    /// <see cref="https://xkcd.com/221/"/>
    double Sample();
    }
