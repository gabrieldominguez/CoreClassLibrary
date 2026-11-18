using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;

namespace CoreClassLibrary.Validation;

/// <summary>
/// Permite determinar cuando un objeto derivado es válido
/// </summary>
/// <remarks>
/// Cuando se ejecuta Validator.TryValidateObject() o Validator.ValidateObject(), se ejecutan los validadores 
/// de las propiedades (que tengan ValidationAttribute), si todos resultaron exitosos, por ultimo se ejecuta Validate().
/// </remarks>
public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo, IDataErrorInfo, IValidatableObject
    {
    /// <summary>
    /// Permite agregar validaciones a nivel de objeto una vez que fueron pasadas las validaciones a nivel de propiedades.
    /// </summary>
    /// <remarks>
    /// Este metodo se ejecuta solo si los validadores de las propiedades (que tengan ValidationAttribute), 
    /// resultaron todos exitosos.
    /// 
    /// En validationContext.Items pueden pasarse un diccionario de objetos que luego puede ser recuperado en el metodo Validate del objeto validado.
    /// 
    /// Si una validacion falla, este metodo no debe lanzar una excepcion, debe agregar un ValidationResult a la lista.
    /// Si la validacion es exitosa no hay que agregar un ValidationResult.
    /// </remarks>
    /// <returns>Lista con los errores de validacion (vacia por default).</returns>
    /// <example>
    ///         <![CDATA[
    ///         public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    ///             {
    ///             List<ValidationResult> results = new List<ValidationResult>();
    /// 
    ///             if (validationContext.Items.ContainsKey("NroLoteValidos"))
    ///                 {
    ///                 List<int> nroLoteValidos = validationContext.Items["NroLoteValidos"] as List<int>;
    /// 
    ///                 if (nroLoteValidos != null && !nroLoteValidos.Contains(this.NroLote))
    ///                     {
    ///                     results.Add(new ValidationResult("El nro lote no esta entre los validos."));
    ///                     }
    ///                 }
    /// 
    ///             if (this.NroTarjeta < this.NroLote)
    ///                 {
    ///                 return new List<ValidationResult>() { new ValidationResult("NroTarjeta menor que NroLote", new string[] { "NroTarjeta", "NroLote" }) };
    ///                 }
    /// 
    ///             return results;
    ///             }
    ///         ]]>
    /// </example>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => [];

    public static void Validate(object instance, object contextItemKey, object contextItem) => ValidatableObject.Validate(instance, new Dictionary<object, object?>() { { contextItemKey, contextItem } });

    public static void Validate(object instance, IDictionary<object, object?>? contextItems = null) => Validator.ValidateObject(instance, new ValidationContext(instance, contextItems), true);

    public static List<ValidationResult> TryValidate(object instance, object contextItemKey, object contextItem) => ValidatableObject.TryValidate(instance, new Dictionary<object, object?>() { { contextItemKey, contextItem } });

    public static List<ValidationResult> TryValidate(object instance, IDictionary<object, object?>? contextItems = null)
        {
        List<ValidationResult> results = [];
        Validator.TryValidateObject(instance, new ValidationContext(instance, contextItems), results, true);
        return results;
        }

    /// <summary>
    /// Valida todas las propiedades del objeto, lanzando un error en la primera que no pase la validacion.
    /// </summary>
    /// <remarks>
    /// Actualiza la propiedad IDataErrorInfo.Error con el mensaje del error lanzado.
    /// </remarks>
    public void Validate()
        {
        try
            {
            ValidatableObject.Validate(this);
            }
        catch (Exception ex)
            {
            this.Error = ex.Message;

            throw;
            }
        }

    /// <summary>
    /// Valida todas las propiedades del objeto.
    /// </summary>
    /// <remarks>
    /// Actualiza la propiedad IDataErrorInfo.Error.
    /// </remarks>
    /// <returns>
    /// Retorna una lista con los errores encontrados.
    /// </returns>
    public List<ValidationResult> TryValidate()
        {
        List<ValidationResult> results = ValidatableObject.TryValidate(this);
        this.Error = results.Select(result => result.ErrorMessage ?? "").ToDelimitedString(Environment.NewLine);
        return results;
        }

    /// <summary>
    /// Valida todas las propiedades del objeto.
    /// </summary>
    /// <remarks>
    /// Actualiza la propiedad IDataErrorInfo.Error.
    /// </remarks>
    /// <returns>
    /// Retorna true si ninguna propiedad dio error.
    /// </returns>
    public virtual bool IsValid() => this.TryValidate().Count == 0;

    protected Type DerivedType => this.GetType();

    public string this[Expression<Func<object>> expression] => this[expression.GetProperty().Name];

    /// <summary>
    /// Gets the error message for the property with the given name using ValidationAttributes.
    /// </summary>
    /// <returns>
    /// The error message for the property. The default is an empty string ("").
    /// </returns>
    public virtual string this[string propertyName] => this.GetErrors(propertyName).Cast<ValidationResult>().Select(result => result.ErrorMessage ?? "").ToDelimitedString(Environment.NewLine);


    private string error = "";
    /// <summary>
    /// Gets or Sets an error message indicating what is wrong with this object.
    /// </summary>
    /// <returns>
    /// An error message indicating what is wrong with this object. The default is an empty string ("").
    /// </returns>
    public string Error
        {
        get => this.error;

        protected set
            {
            if (this.error != value)
                {
                this.error = value;
                this.RaisePropertyChanged(nameof(this.Error));
                }
            }
        }


    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e) => this.ErrorsChanged?.Invoke(this, e);

    public void RaiseErrorsChanged(Expression<Func<object>> expression) => this.RaiseErrorsChanged(expression.GetProperty().Name);

    public void RaiseErrorsChanged(string propertyName) => this.OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));

    public IEnumerable GetErrors(Expression<Func<object>> propertyExpresssion) => this.GetErrors(propertyExpresssion.GetProperty().Name);

    public IEnumerable GetErrors(string? propertyName)
        {
        List<ValidationResult> results = [];

        if (propertyName != null)
            {
            Validator.TryValidateProperty(this.DerivedType.GetRuntimeProperty(propertyName)?.GetValue(this), new ValidationContext(this, null) { MemberName = propertyName }, results);
            }

        return results;
        }

    public bool HasErrors => this.error != "";
    }
