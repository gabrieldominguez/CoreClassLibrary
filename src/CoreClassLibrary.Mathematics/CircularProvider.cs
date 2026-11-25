
namespace CoreClassLibrary.Mathematics;

/// <summary>
/// Retorna los valores pasados en el constructor.
/// </summary>
/// <remarks>
/// Para hacer pruebas repetibles.
/// Una vez que se llego al último valor, vuelve al primero.
/// </remarks>
public class CircularProvider<T>(IReadOnlyList<T> samples) : ISampleProvider<T>
    {
    private int i = 0;

    public void Initialize()
        => i = 0;

    public T GetNext()
        {
        T t = samples[i];
        i = (i + 1) % samples.Count;
        return t;
        }
    }
