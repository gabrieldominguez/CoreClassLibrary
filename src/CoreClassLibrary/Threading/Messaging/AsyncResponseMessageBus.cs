using System.Collections.Concurrent;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.WeakMethods;

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
///        MessagingResponseService.Subscribe<IDeudaViewModel>("TarjetaActivaDeuda", this.Initialize);
///        ...
///        MessagingResponseService.Publish<IDeudaViewModel>("TarjetaActivaDeuda", cuentaViewModel);
/// ]]>
/// </example>
/// <see cref="https://github.com/jbogard/MediatR"/>
public class AsyncResponseMessageBus
{
    private static readonly Lazy<AsyncResponseMessageBus> instance = new Lazy<AsyncResponseMessageBus>();

    public static AsyncResponseMessageBus Instance => instance.Value;

    private readonly ConcurrentDictionary<string, WeakMethodReference> funcs = [];

    /// <summary>
    /// Registra un método para recibir mensajes.
    /// </summary>
    /// <param name="func">Método que se ejecutará al recibir el mensaje.</param>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        MessagingResponseService.Subscribe<IDeudaViewModel>(this.Initialize);
    /// ]]>
    /// </example>
    public void Subscribe<T, TResult>(Func<T, CancellationToken, Task<TResult>> func) => this.Subscribe(typeof(T).FullName ?? typeof(T).Name, func);

    /// <summary>
    /// Registra un método para recibir mensajes.
    /// </summary>
    /// <param name="messageName">Nombre del mensaje a recibir.</param>
    /// <param name="func">Método que se ejecutará al recibir el mensaje.</param>
    /// <remarks>
    /// Los métodos anónimos no pueden desregistrarse.
    /// Si se necesitan desregistrar, usar métodos de intancia o estáticos
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        MessagingResponseService.Subscribe<IDeudaViewModel>("TarjetaActivaDeuda", this.Initialize);
    /// ]]>
    /// </example>
    public void Subscribe<T, TResult>(string messageName, Func<T, CancellationToken, Task<TResult>> func) => funcs.GetOrAdd(messageName, key => new WeakFunc<T, CancellationToken, Task<TResult>>(func));

    /// <summary>
    /// Desregistra un método.
    /// </summary>
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// <remarks>
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        MessagingResponseService.Unregister<IDeudaViewModel>();
    /// ]]>
    /// </example>
    public void Unregister<T>() => this.Unregister(typeof(T).FullName ?? typeof(T).Name);

    /// <summary>
    /// Desregistra un mensaje.
    /// </summary>
    public void Unregister(string messageName) => funcs.TryRemove(messageName);

    /// <summary>
    /// Envía un mensaje a todos los detinatarios registrados.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <remarks>
    /// Se recomienda explicitar el tipo T si es una interface o un tipo base.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///        MessagingResponseService.Publish<IDeudaViewModel>(cuentaViewModel);
    /// ]]>
    /// </example>
    public Task<TResult> PublishAsync<T, TResult>(T message)
        => this.PublishAsync<T, TResult>(typeof(T).FullName ?? typeof(T).Name, message);

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
    ///        Response response = MessagingResponseService.Publish<IDeudaViewModel, Response>("TarjetaActivaDeuda", cuentaViewModel);
    /// ]]>
    /// </example>
    public Task<TResult> PublishAsync<T, TResult>(string messageName, T message, CancellationToken cancellationToken = default)
    {
        if (funcs.TryGetValue(messageName, out WeakMethodReference? weakMethod))
        {
            if (weakMethod.IsAlive)
            {
                return ((WeakFunc<T, CancellationToken, Task<TResult>>)weakMethod).Func(message, cancellationToken);
            }
            else
            {
                this.Unregister(messageName);
            }
        }

        throw new InvalidOperationException(string.Concat("No func for message ", messageName, " has been registered."));
    }
}
