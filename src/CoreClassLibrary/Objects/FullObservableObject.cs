using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

//Event Design: http://msdn.microsoft.com/en-us/library/ms229011.aspx

/*
 * In the Box – MVVM Training
 * http://karlshifflett.wordpress.com/2010/11/07/in-the-box-ndash-mvvm-training/
 */

/*
 FOR WIN8: the use of the CallerMemberName attribute.
 * 
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")

        protected void RaisePropertyChanging([CallerMemberName] string propertyName = "")
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
 */

namespace CoreClassLibrary.Objects;

/// <summary>
/// Agrega funcionalidad para clases que implementen INotifyPropertyChanging y/o INotifyPropertyChanged.
/// </summary>
/// <remarks>
/// RaisePropertyChang[ing/ed]: dispara el evento PropertyChanging para la propiedad especificada.
/// AddPropertyChang[ing/ed]Dependency: hace que cuando una propiedad que depende de otra, se dispare el evento para las dos cuando se mofique la 2da.
/// </remarks>
/// <example>
/// Para disparar el evento que indica que se modifico la propiedad Id:
///     this.RaisePropertyChanged(() => this.Id);
/// 
/// En el constructor del viewmodel, hace que la propiedad BaseDirectory dependa de la propiedad Id:
///     this.AddPropertyChangedDependency(() => this.Id, () => this.BaseDirectory);
/// </example>
public abstract class FullObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
    /// <summary>
    /// Setea una propiedad, disparando los eventos correspodientes.
    /// </summary>
    /// <returns>True si la propiedad fue modificada</returns>
    /// <example>
    /// private byte[] _name;
    /// public byte[] Name
    ///     {
    ///     get { return this._name; }
    ///     set { this.SetProperty(ref _name, value, () => this.Name); }
    ///     }
    /// </example>
    protected bool SetProperty<T>(ref T field, T value, Expression<Func<T>> expression)
        => this.SetProperty(ref field, value, expression.GetProperty().Name);

    protected bool SetProperty<T>(ref T field, T value, string propertyName)
        {
        this.VerifyPropertyName(propertyName);

        if (Object.Equals(field, value))
            {
            return false;
            }

        this.RaisePropertyChanging(propertyName);
        field = value;
        this.RaisePropertyChanged(propertyName);

        return true;
        }

    public event PropertyChangingEventHandler? PropertyChanging;
    protected virtual void OnPropertyChanging(PropertyChangingEventArgs e) => this.PropertyChanging?.Invoke(this, e);

    protected void RaisePropertyChanging<T>(Expression<Func<T>> expression) => this.RaisePropertyChanging(expression.GetProperty().Name);

    protected void RaisePropertyChanging(string propertyName)
        {
        this.VerifyPropertyName(propertyName);
        this.OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

    protected void AddPropertyChangingDependency<S, T>(Expression<Func<S>> sourceExpression, Expression<Func<T>> targetExpression) => this.AddPropertyChangingDependency(sourceExpression.GetProperty().Name, targetExpression.GetProperty().Name);

    protected void AddPropertyChangingDependency(string sourcePropertyName, string targetPropertyName)
        {
        this.VerifyPropertyName(sourcePropertyName);
        this.VerifyPropertyName(targetPropertyName);

        this.PropertyChanging += (s, args) =>
        {
            if (args.PropertyName == sourcePropertyName)
                {
                this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(targetPropertyName));
                }
        };
        }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

    protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) => this.RaisePropertyChanged(expression.GetProperty().Name);

    protected void RaisePropertyChanged(string propertyName)
        {
        this.VerifyPropertyName(propertyName);
        this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

    protected void AddPropertyChangedDependency<S, T>(Expression<Func<S>> sourceExpression, Expression<Func<T>> targetExpression) => this.AddPropertyChangedDependency(sourceExpression.GetProperty().Name, targetExpression.GetProperty().Name);

    protected void AddPropertyChangedDependency(string sourcePropertyName, string targetPropertyName)
        {
        this.VerifyPropertyName(sourcePropertyName);
        this.VerifyPropertyName(targetPropertyName);

        this.PropertyChanged += (s, args) =>
        {
            if (args.PropertyName == sourcePropertyName)
                {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(targetPropertyName));
                }
        };
        }

    /// <summary>
    /// Warns the developer if this Object does not have a public property with
    /// the specified name. This method does not exist in a Release build.
    /// </summary>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName)
        {
        // verify that the property name matches a real,  
        // public, instance property on this Object.
        if (this.GetType().GetTypeInfo().GetProperty(propertyName) == null)
            {
            Debug.Fail("Invalid property name: " + propertyName);
            }
        }
    }
