#pragma warning disable IDE0028 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.WeakMethods;
using System.Collections.Concurrent;

namespace CoreClassLibrary.Threading.Messaging;

/// <summary>
/// Permite enviar mensajes entre objetos.
/// </summary>
/// <remarks>
/// El tipo del mensaje a enviar debe coincidir exactamente el tipo registrado,
/// por lo que se recomienda explicitar el tipo T 
/// tanto en la registracion como en el envio del mensaje si es una interface o un tipo base.
/// </remarks>
/// <example>
/// <![CDATA[
///        messageBus.Subscribe<IDeudaViewModel>("TarjetaActivaDeuda", this.Initialize);
///        ...
///        messageBus.Publish<IDeudaViewModel>("TarjetaActivaDeuda", cuentaViewModel);
/// ]]>
/// </example>
/// <see cref="https://github.com/jbogard/MediatR"/>
public class AsyncMessageBus
{
    private static readonly Lazy<AsyncMessageBus> instance = new Lazy<AsyncMessageBus>();

    public static AsyncMessageBus Instance => instance.Value;

    private readonly ConcurrentDictionary<string, List<WeakMethodReference>> actions = [];

    /// <summary>
    /// Registra un método para recibir mensajes.
    /// </summary>
    /// <param name="action">Método que se ejecutará al recibir el mensaje.</param>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messageBus.Subscribe<IDeudaViewModel>(this.Initialize);
    /// ]]>
    /// </example>
    public void Subscribe<T>(Func<T, CancellationToken, Task> action) => this.Subscribe(typeof(T).FullName ?? typeof(T).Name, action);

    /// <summary>
    /// Registra un método para recibir mensajes.
    /// </summary>
    /// <param name="messageName">Nombre del mensaje a recibir.</param>
    /// <param name="action">Método que se ejecutará al recibir el mensaje.</param>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messageBus.Subscribe<IDeudaViewModel>("TarjetaActivaDeuda", this.Initialize);
    /// ]]>
    /// </example>
    public void Subscribe<T>(string messageName, Func<T, CancellationToken, Task> action) => actions.GetOrAdd(messageName, key => new List<WeakMethodReference>()).AddUnique(new WeakFunc<T, CancellationToken, Task>(action));

    /// <summary>
    /// Desregistra un método.
    /// </summary>
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// <remarks>
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messageBus.Unsubscribe<IDeudaViewModel>(this.Initialize);
    /// ]]>
    /// </example>
    public void Unsubscribe<T>(Func<T, CancellationToken, Task> action) => this.Unsubscribe(typeof(T).FullName ?? typeof(T).Name, action);

    /// <summary>
    /// Desregistra un método.
    /// </summary>
    /// <param name="messageName">Nombre del mensaje a recibir.</param>
    /// <param name="action">Método que se ejecutará al recibir el mensaje.</param>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messageBus.Unsubscribe<IDeudaViewModel>("TarjetaActivaDeuda", this.Initialize);
    /// ]]>
    /// </example>
    public void Unsubscribe<T>(string messageName, Func<T, CancellationToken, Task> action)
        {   
        if (actions.TryGetValue(messageName, out List<WeakMethodReference>? weakMethods))
            {
            weakMethods.Remove(new WeakFunc<T, CancellationToken, Task>(action));

            if (weakMethods.Count == 0)
                {
                this.Unsubscribe(messageName);
                }
            }
        }

    /// <summary>
    /// Desregistra un mensaje.
    /// </summary>
    public void Unsubscribe(string messageName) => actions.TryRemove(messageName);

    /// <summary>
    /// Envía un mensaje a todos los detinatarios registrados.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <remarks>
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messageBus.Publish<IDeudaViewModel>(cuentaViewModel);
    /// ]]>
    /// </example>
    public Task PublishAsync<T>(T message) => this.PublishAsync(typeof(T).FullName ?? typeof(T).Name, message);

    /// <summary>
    /// Envía un mensaje a todos los detinatarios registrados.
    /// </summary>
    /// <param name="messageName">Nombre del mensaje a enviar.</param>
    /// <param name="message">Mensaje a enviar.</param>
    /// <remarks>
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        messageBus.Publish<IDeudaViewModel>("TarjetaActivaDeuda", cuentaViewModel);
    /// ]]>
    /// </example>
    public async Task PublishAsync<T>(string messageName, T message, CancellationToken cancellationToken = default)
    {
        if (actions.TryGetValue(messageName, out List<WeakMethodReference>? weakMethods))
        {
            bool hasDeadMethods = false;
            foreach (WeakMethodReference weakMethod in weakMethods)
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
                weakMethods.RemoveAll(method => !method.IsAlive);

                if (weakMethods.Count == 0)
                {
                    this.Unsubscribe(messageName);
                }
            }
        }
    }
}
