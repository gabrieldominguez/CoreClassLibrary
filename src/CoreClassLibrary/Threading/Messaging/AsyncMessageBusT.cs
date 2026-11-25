using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.WeakMethods;

namespace CoreClassLibrary.Threading.Messaging;

/// <summary>
/// Permite enviar mensajes de un tipo especificado entre objetos.
/// </summary>
/// <example>
/// <![CDATA[
///        messagingService<IDeudaViewModel>.Subscribe("TarjetaActivaDeuda", this.Initialize);
///        ...
///        messagingService<IDeudaViewModel>.Publish("TarjetaActivaDeuda", cuentaViewModel);
/// ]]>
/// </example>
public class AsyncMessagingService<T>
{
    private readonly List<WeakMethodReference> actions = [];

    #region Subscribe
    /// <summary>
    /// Registra un método para recibir mensajes.
    /// </summary>
    /// <param name="action">Método que se ejecutará al recibir el mensaje.</param>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messagingService<IDeudaViewModel>.Subscribe(this.Initialize);
    /// ]]>
    /// </example>
    public void Subscribe(Func<T, CancellationToken, Task> action) => actions.AddUnique(new WeakFunc<T, CancellationToken, Task>(action));
    #endregion

    #region Unregister
    /// <summary>
    /// Desregistra un método.
    /// </summary>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messagingService.Unregister<IDeudaViewModel>(this.Initialize);
    /// ]]>
    /// </example>
    public void Unregister(Func<T, CancellationToken, Task> action) => actions.Remove(new WeakFunc<T, CancellationToken, Task>(action));
    #endregion

    #region Publish
    /// <summary>
    /// Envía un mensaje a todos los detinatarios registrados.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <example>
    /// <![CDATA[
    ///        messagingService<IDeudaViewModel>.Publish(cuentaViewModel);
    /// ]]>
    /// </example>
    public async Task PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        bool hasDeadMethods = false;
        foreach (WeakMethodReference weakMethod in actions)
        {
            if (weakMethod.IsAlive)
            {
                await ((weakMethod as WeakFunc<T, CancellationToken, Task>)?.Func(message, cancellationToken) ?? Task.CompletedTask);
            }
            else
            {
                hasDeadMethods = true;
            }
        }

        if (hasDeadMethods)
        {
            actions.RemoveAll(method => !method.IsAlive);
        }
    }
    #endregion
}
