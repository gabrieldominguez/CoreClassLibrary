using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.WeakMethods;

namespace CoreClassLibrary.Objects.MessagingServices;

/// <summary>
/// Permite enviar mensajes de un tipo especificado entre objetos.
/// </summary>
/// <example>
/// <![CDATA[
///        messagingService<IDeudaViewModel>.Register("TarjetaActivaDeuda", this.Initialize);
///        ...
///        messagingService<IDeudaViewModel>.Send("TarjetaActivaDeuda", cuentaViewModel);
/// ]]>
/// </example>
public class MessagingService<T>
{
    private readonly List<WeakMethodReference> actions = [];

    #region Register
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
    ///        messagingService<IDeudaViewModel>.Register(this.Initialize);
    /// ]]>
    /// </example>
    public void Register(Action<T> action) => actions.AddUnique(new WeakAction<T>(action));
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
    public void Unregister(Action<T> action) => actions.Remove(new WeakAction<T>(action));
    #endregion

    #region Send
    /// <summary>
    /// Envía un mensaje a todos los detinatarios registrados.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <example>
    /// <![CDATA[
    ///        messagingService<IDeudaViewModel>.Send(cuentaViewModel);
    /// ]]>
    /// </example>
    public void Send(T message)
    {
        bool hasDeadMethods = false;
        foreach (WeakMethodReference weakMethod in actions)
        {
            if (weakMethod.IsAlive)
            {
                (weakMethod as WeakAction<T>)?.Action(message);
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
