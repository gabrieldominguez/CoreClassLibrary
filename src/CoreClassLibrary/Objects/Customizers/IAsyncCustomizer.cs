namespace CoreClassLibrary.Objects.Customizers;

/// <summary>
/// Configura un objeto asincronicamente.
/// </summary>
/// <see cref="https://www.linkedin.com/pulse/20140610191154-4746170-configuration-vs-customization-when-and-why-would-i-implement-each"/>
/// <see cref="https://www.mitratech.com/resource-hub/blog/configuration-vs-customization-whats-difference-matter/"/>
public interface IAsyncCustomizer<T> where T: class
    {
    /// <summary>
    /// Configura un objeto.
    /// </summary>
    /// <param name="value">Objeto a configurar</param>
    /// <returns>True si el objecto fue configurado exitosamente.</returns>
    Task<bool> CustomizeAsync(T value);
    }
