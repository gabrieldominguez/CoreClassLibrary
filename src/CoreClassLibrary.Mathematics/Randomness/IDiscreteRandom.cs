
namespace CoreClassLibrary.Mathematics.Randomness;

/// <summary>
/// Interface que deben implementar los generadores de muestras discretas.
/// </summary>
public interface IDiscreteRandom
    {
    /// <summary>
    /// Generate a random unsigned integer.
    /// </summary>
    /// <returns>An int [0..Int32.MaxValue)</returns>
    uint DiscreteSample();
    }
