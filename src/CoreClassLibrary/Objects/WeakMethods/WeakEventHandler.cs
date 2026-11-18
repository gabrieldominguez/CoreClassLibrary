namespace CoreClassLibrary.Objects.WeakMethods;

/// <summary>
/// WeakReference Event Handlers
/// </summary>
/// <remarks>
/// http://www.paulstovell.com/weakevents
/// </remarks>
/// <example>
/// <![CDATA[
/// When subscribing to events, instead of writing:
///     alarm.Beep += Alarm_Beeped;
/// Just write:
///     alarm.Beeped += new WeakEventHandler<AlarmEventArgs>(Alarm_Beeped).Handler;
/// ]]>
/// </example>
public sealed class WeakEventHandler<TEventArgs>(EventHandler<TEventArgs> callback) : WeakMethodReference(callback) where TEventArgs : EventArgs
    {
    public void Handler(object sender, TEventArgs e)
        {
        if (this.IsAlive)
            {
            this.CreateDelegate<Action<object, TEventArgs>>()?.Invoke(sender, e);
            }
        }
    }
