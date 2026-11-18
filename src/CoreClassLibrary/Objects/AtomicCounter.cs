// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// https://github.com/microsoft/reverse-proxy/blob/main/src/ReverseProxy/Utilities/AtomicCounter.cs

namespace CoreClassLibrary.Objects;

public sealed class AtomicCounter
    {
    private int value = 0;

    /// <summary>
    /// Gets the current value of the counter.
    /// </summary>
    public int Value
        {
        get => Volatile.Read(ref value);
        set => Volatile.Write(ref this.value, value);
        }

    /// <summary>
    /// Atomically increments the counter value by 1.
    /// </summary>
    public int Increment() => Interlocked.Increment(ref value);

    /// <summary>
    /// Atomically decrements the counter value by 1.
    /// </summary>
    public int Decrement() => Interlocked.Decrement(ref value);

    /// <summary>
    /// Atomically resets the counter value to 0.
    /// </summary>
    public void Reset() => Interlocked.Exchange(ref value, 0);
    }
