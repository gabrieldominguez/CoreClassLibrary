using System.ComponentModel;
using System.Linq.Expressions;

//Event Design: http://msdn.microsoft.com/en-us/library/ms229011.aspx

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
public abstract class ObservableObject : INotifyPropertyChanged //, INotifyPropertyChanging
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
        if (Object.Equals(field, value))
            {
            return false;
            }

        field = value;
        this.RaisePropertyChanged(propertyName);

        return true;
        }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);/*
        if (this.PropertyChanged != null)
            {
            this.PropertyChanged(this, e);
            }
        */

    protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) => this.RaisePropertyChanged(expression.GetProperty().Name);

    protected void RaisePropertyChanged(string propertyName) => this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

    protected void AddPropertyChangedDependency<S, T>(Expression<Func<S>> sourceExpression, Expression<Func<T>> targetExpression) => this.AddPropertyChangedDependency(sourceExpression.GetProperty().Name, targetExpression.GetProperty().Name);

    protected void AddPropertyChangedDependency(string sourcePropertyName, string targetPropertyName) => this.PropertyChanged += (s, args) =>
                                                                                                              {
                                                                                                                  if (args.PropertyName == sourcePropertyName)
                                                                                                                      {
                                                                                                                      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(targetPropertyName));
                                                                                                                      }
                                                                                                              };
    }
