using System.Diagnostics;

namespace CoreClassLibrary.Objects.Scopes;

/// <summary>
/// Crea Activity en un scope.
/// </summary>
[Obsolete("Use using Activity activity = new Activity(operationName).Start(); instead")]
public sealed class ActivityScope : IDisposable
    {
    private readonly Activity activity;

    public ActivityScope(string operationName = "")
        {
        (this.activity = new Activity(operationName)).Start(); 
        }
    
    /// <summary>
    /// Termina el scope, deteniendo el Activity.
    /// </summary>
    public void Dispose()
        {
        this.activity.Dispose();
        }
    }
