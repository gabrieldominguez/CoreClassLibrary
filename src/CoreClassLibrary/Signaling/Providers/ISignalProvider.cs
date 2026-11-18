namespace CoreClassLibrary.Signaling.Providers;

/// <summary>
/// Permite emitir un evento para señalizar un suceso.
/// </summary>
public interface ISignalProvider
    {
    //Action Signaled { get; }
    event EventHandler Signaled;
    }
