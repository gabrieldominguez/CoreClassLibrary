namespace CoreClassLibrary.Objects.Disposables;

/// <summary>
/// Implements IDisposable as a base for classes that encapsulate unmanaged resources.
/// </summary>
/// <remarks>
/// Recommended design pattern for implementing a Dispose method for classes that encapsulate unmanaged resources.
/// ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.VisualStudio.v80.en/dv_fxadvance/html/eb4e1af0-3b48-4fbc-ad4e-fc2f64138bf9.htm
///
/// This class has a destructor.
/// Implementing Finalize methods or destructors can have a negative impact on performance and you should avoid using them unnecessarily.
///
/// Calling Finalize
/// http://social.msdn.microsoft.com/forums/en-US/csharplanguage/thread/5f591fa1-a994-4ae6-94de-27ec069dc116/
///
/// Dispose vs Dispose(bool)
/// http://stackoverflow.com/questions/5225168/dispose-vs-disposebool
/// </remarks>
public abstract class DisposableObject : IDisposable
    {
    public event EventHandler? Disposing;
    public virtual void OnDisposing(EventArgs e) => this.Disposing?.Invoke(this, e);

    public event EventHandler? Disposed;
    public virtual void OnDisposed(EventArgs e) => this.Disposed?.Invoke(this, e);

    /// <summary>
    /// Track whether Dispose is been executed.
    /// </summary>
    protected bool IsDisposing { get; private set; }

    /// <summary>
    /// Track whether Dispose has been called.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <summary>
    /// Releases all resources held by the current object
    /// </summary>
    /// <remarks>
    /// Implement IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </remarks>
    public void Dispose()
        {
        this.Dispose(true);

        // Take yourself off the Finalization queue 
        // to prevent finalization code for this object
        // from executing a second time.
        // https://docs.microsoft.com/es-es/dotnet/fundamentals/code-analysis/quality-rules/ca1816
        GC.SuppressFinalize(this);
        }

    /// <summary>
    /// Releases all resources held by the current object
    /// </summary>
    /// <param name="disposing">
    /// Dispose(bool disposing) Starts in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the
    /// runtime from inside the finalizer and you should not reference
    /// other objects. Only unmanaged resources can be disposed.
    /// </param>
    /// <remarks>
    /// A call to Dispose(false) should only clean up native resources.
    /// A call to Dispose(true) should clean up both managed and native resources.
    /// </remarks>
    /// <see cref="http://stackoverflow.com/questions/5225168/dispose-vs-disposebool"/>
    protected virtual void Dispose(bool disposing)
        {
        // Check to see if Dispose has already been called.
        // If an object's Dispose method is called more than once, the object must ignore all calls after the first one. The object must not throw an exception if its Dispose method is called multiple times. Instance methods other than Dispose can throw an ObjectDisposedException when resources are already disposed.
        if (!this.IsDisposed)
            {
            this.IsDisposing = true;
            this.OnDisposing(EventArgs.Empty);

            // If disposing is true, dispose all managed and unmanaged resources.
            if (disposing)
                {
                this.DisposeManaged();
                }

            // If disposing is false, only the following code is executed.
            this.DisposeUnmanaged();

            this.IsDisposing = false;

            this.IsDisposed = true;
            this.OnDisposed(EventArgs.Empty);
            }
        }

    /// <summary>
    /// Use C# destructor syntax for finalization code.
    /// </summary>
    /// <remarks>
    /// This destructor will run only if the Dispose method does not get called.
    /// It gives your base class the opportunity to finalize.
    /// Do not provide destructors in types derived from this class.
    ///
    /// Implementing Finalize methods or destructors can have a negative impact on performance and you should avoid using them unnecessarily.
    /// </remarks>
    ~DisposableObject()
        {
        // Do not re-create Dispose clean-up code here.
        // Calling Dispose(false) is optimal in terms of readability and maintainability.
        this.Dispose(false);
        }

    /// <summary>
    /// For types where calling a Close method is more natural than calling a Dispose method.
    /// </summary>
    /// <remarks>
    /// Do not make this method virtual.
    /// A derived class should not be allowed to override this method.
    /// </remarks>
    public void Close() => this.Dispose();

    /// <summary>
    /// Dispose managed resources.
    /// </summary>
    /// <remarks>
    /// Call the appropriate methods to clean up unmanaged resources here.
    /// Set large fields to null.
    /// </remarks>
    protected virtual void DisposeManaged() {}

    /// <summary>
    /// Dispose unmanaged resources.
    /// </summary>
    /// <remarks>
    /// Call the appropriate methods to clean up managed resources here.
    /// Set large fields to null.
    /// </remarks>
    protected virtual void DisposeUnmanaged() {}
    }
