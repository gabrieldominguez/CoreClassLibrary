using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using CoreClassLibrary.Exceptions;
using CoreClassLibrary.Extensions;


// Based on Delta<T> extracted from 
// packages\Microsoft.AspNet.WebApi.OData.5.0.0\lib\net45\System.Web.Http.OData.dll
// System.Web.Http.OData, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35

namespace CoreClassLibrary.Objects.DLR;

/// <summary>
/// A class the tracks changes (i.e. the delta) for a particular TEntity />.
/// </summary>
/// <typeparam name="TEntity">TEntityType is the base type of entity this delta tracks changes for.</typeparam>
/// <remarks>
/// <![CDATA[
/// Esta clase es similar a ExpandoObject y tambien puede usarse con dynamic, pero a diferencia de la primera
/// si al momento de setear una propiedad esta no existe o es de un tipo incompatible con el del valor que se intenta setear,
/// se lanzará una excepción.
///
/// One of the great features of the Microsoft ASP.NET WebAPI OData package is a little dynamic proxy object called Delta<T>.
/// OData use ODataMediaTypeFormatter to parse a Delta<T> object.
/// ]]>
/// </remarks>
/// <see cref="http://stackoverflow.com/questions/14729249/how-to-use-deltat-from-microsoft-asp-net-web-api-odata-with-code-first-jsonmed"/>
/// <see cref="http://www.strathweb.com/2013/01/easy-asp-net-web-api-resource-updates-with-delta/#comments"/>
/// <example>
/// <![CDATA[
///             const int id = 1388;
///             IDeudaRepository repository = RepositoryTests.CreateDeudaRepository();
///
///             Deuda cuenta = repository.Get(id);
///             if (cuenta != null)
///                 {
///                 dynamic delta = new Delta<Deuda>();
///
///                 delta.NombreFantasia = "HHH";
///                 delta.RazonSocial = "hhh";
///
///                 repository.Patch(new object[] { id }, (Delta<Deuda>)delta);
///                 }
/// ]]>
/// <![CDATA[
///             const int id = 1388;
///             IDeudaRepository repository = RepositoryTests.CreateDeudaRepository();
///
///             Deuda cuenta = repository.Get(id);
///             if (cuenta != null)
///                 {
///                 Delta<Deuda> delta = new Delta<Deuda>();
///
///                 delta.TrySetPropertyValue("NombreFantasia", "BBB");
///                 delta.TrySetPropertyValue("RazonSocial", "bbb");
///
///                 repository.Patch(new object[] { id }, delta);
///                 }
/// ]]>
/// </example>
public class Delta<TEntity> : TypedDelta where TEntity : class
    {
    private class CompiledPropertyAccessor<TEntityType> : PropertyAccessor<TEntityType>
        {
        private readonly Action<TEntityType, object> _setter;
        private readonly Func<TEntityType, object> _getter;

        public CompiledPropertyAccessor(PropertyInfo property): base(property)
            {
            this._setter = CompiledPropertyAccessor<TEntityType>.MakeSetter(base.Property);
            this._getter = CompiledPropertyAccessor<TEntityType>.MakeGetter(base.Property);
            }

        public override object GetValue(TEntityType entity) => this._getter(entity);

        public override void SetValue(TEntityType entity, object value) => this._setter(entity, value);

        private static Action<TEntityType, object> MakeSetter(PropertyInfo property)
            {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntityType));
            ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object));

            return Expression.Lambda<Action<TEntityType, object>>(
                Expression.Assign(
                    Expression.Property(Expression.TypeAs(parameterExpression, property.DeclaringType!), property),
                    Expression.Convert(parameterExpression2, property.PropertyType)
                    ),
                [
				        parameterExpression,
				        parameterExpression2
			            ]).Compile();
            }

        private static Func<TEntityType, object> MakeGetter(PropertyInfo property)
            {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntityType));

            return Expression.Lambda<Func<TEntityType, object>>(
                Expression.Convert(Expression.Property(Expression.TypeAs(parameterExpression, property.DeclaringType!), property), typeof(object)),
                [
				        parameterExpression
			            ]).Compile();
            }
        }

    private abstract class PropertyAccessor<TEntityType>
        {
        public PropertyInfo Property { get; }

        protected PropertyAccessor(PropertyInfo property)
            {
            this.Property = property;

            if (this.Property.GetMethod == null || this.Property.SetMethod == null)
                {
                throw new ArgumentException("Property Must Have Public Getter And Setter", nameof(property));
                }
            }

        public void Copy(TEntityType from, TEntityType to) => this.SetValue(to, this.GetValue(from));

        public abstract object GetValue(TEntityType entity);

        public abstract void SetValue(TEntityType entity, object value);
        }

    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyAccessor<TEntity>>> _propertyCache = [];
    private readonly Type _entityType;
    private TEntity _entity;
    private HashSet<string> _changedProperties;
    private Dictionary<string, PropertyAccessor<TEntity>> _propertiesThatExist;

    /// <summary>
    /// Gets the actual type of the entity for which the changes are tracked.
    /// </summary>
    /// <returns>The actual type of the entity for which the changes are tracked.</returns>
    public override Type EntityType => this._entityType;

    public override Type ExpectedClrType => typeof(TEntity);

    /// <summary>
    /// Initializes a new instance of <see cref="Delta" />.
    /// </summary>
    public Delta(): this(typeof(TEntity)) { }

    /// <summary>
    /// Initializes a new instance of <see cref="Delta" />.
    /// </summary>
    /// <param name="entityType">The derived entity type for which the changes would be tracked. <paramref name="entityType" /> should be assignable to instances of TEntity />.</param>
    private Delta(Type entityType)
        {
        if (!typeof(TEntity).GetTypeInfo().IsAssignableFrom(entityType.GetTypeInfo()))
            {
            throw new InvalidOperationException($"Delta Entity Type Not Assignable: {entityType.FullName}");
            }

        this._entityType = entityType;
        this._entity = (TEntity)(Activator.CreateInstance(entityType) ?? throw new InvalidOperationException($"Cannot create Delta Entity {entityType.FullName}"));
        this._changedProperties = [];
        this._propertiesThatExist = this.InitializePropertiesThatExist();
        }

    /// <summary>
    /// Clears the Delta and resets the underlying Entity.
    /// </summary>
    public override void Clear()
        {
        this._entity = (TEntity)(Activator.CreateInstance(this._entityType) ?? throw new InvalidOperationException($"Cannot create Delta Entity {this._entityType.FullName}"));
        this._changedProperties = [];
        this._propertiesThatExist = this.InitializePropertiesThatExist();
        }

    /// <summary>
    /// Attempts to set the property called <paramref name="name" /> to the <paramref name="value" /> specified.
    /// </summary>
    /// <returns>true if successful; otherwise, false.</returns>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The new value of the property.</param>
    public override bool TrySetPropertyValue(string name, object? value)
        {
        if (!_propertiesThatExist.TryGetValue(name, out Delta<TEntity>.PropertyAccessor<TEntity>? propertyAccessor))
            {
            return false;
            }

        if (value == null && !propertyAccessor.Property.PropertyType.IsNullable())
            {
            return false;
            }

        if (value != null && !propertyAccessor.Property.PropertyType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
            if (propertyAccessor.Property.PropertyType == typeof(int) && value is long intValue && intValue <= int.MaxValue)
                {
                propertyAccessor.SetValue(this._entity, (int)intValue);
                this._changedProperties.Add(name);
                return true;
                }

            if (value is string strValue)
                {
                if (propertyAccessor.Property.PropertyType == typeof(Byte[]))
                    {
                    try
                        {
                        propertyAccessor.SetValue(this._entity, Convert.FromBase64String(strValue));
                        this._changedProperties.Add(name);
                        return true;
                        }
                    catch { }

                    return false;
                    }

                if (propertyAccessor.Property.PropertyType == typeof(Guid))
                    {
                    if (Guid.TryParse(strValue, out Guid guid))
                        {
                        propertyAccessor.SetValue(this._entity, guid);
                        this._changedProperties.Add(name);
                        return true;
                        }
                    else
                        {
                        return false;
                        }
                    }
                }

            return false;
            }

        propertyAccessor.SetValue(this._entity, value!);
        this._changedProperties.Add(name);
        return true;
        }

    /// <summary>
    /// Attempts to get the value of the property called <paramref name="name" /> from the underlying entity.
    /// </summary>
    /// <returns>true if the property was found.</returns>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value of the property.</param>
    public override bool TryGetPropertyValue(string name, [MaybeNullWhen(returnValue: false)] out object? value)
        {
        if (_propertiesThatExist.TryGetValue(name, out Delta<TEntity>.PropertyAccessor<TEntity>? propertyAccessor))
            {
            value = propertyAccessor.GetValue(this._entity);
            return true;
            }
        else
            {
            value = null;
            return false;
            }
        }

    /// <summary>
    /// Attempts to get the <see cref="T:System.Type" /> of the property called <paramref name="name" /> from the underlying entity.
    /// </summary>
    /// <returns>true if the property was found; otherwise, false.</returns>
    /// <param name="name">The name of the property.</param>
    /// <param name="type">The type of the property.</param>
    public override bool TryGetPropertyType(string name, [MaybeNullWhen(returnValue: false)] out Type? type)
        {
        if (this._propertiesThatExist.TryGetValue(name, out PropertyAccessor<TEntity>? propertyAccessor))
            {
            type = propertyAccessor.Property.PropertyType;
            return true;
            }
        else
            {
            type = null;
            return false;
            }
        }

    /// <summary>
    /// Returns the <see cref="Delta" /> instance that holds all the changes (and original values) being tracked by this Delta.
    /// </summary>
    /// <returns>The <see cref="Delta" /> istance.</returns>
    public TEntity GetEntity() => this._entity;

    /// <summary>
    /// Returns the Properties that have been modified through this Delta as an enumeration of property names.
    /// </summary>
    /// <returns>The property names.</returns>
    public override IEnumerable<string> GetChangedPropertyNames() => this._changedProperties;

    /// <summary>
    /// Returns the properties that have not been modified through this Delta as an enumeration of property names.
    /// </summary>
    /// <returns>The properties that have not been modified through this Delta as an enumeration of property names.</returns>
    public override IEnumerable<string> GetUnchangedPropertyNames() => this._propertiesThatExist.Keys.Except(this.GetChangedPropertyNames());

    /// <summary>
    /// Copies the changed property values from the underlying entity (accessible via Delta.GetEntity to the <paramref name="original" /> entity.
    /// </summary>
    /// <param name="original">The entity to be updated.</param>
    public void CopyChangedValues(TEntity original)
        {
        if (!this._entityType.GetTypeInfo().IsAssignableFrom(original.GetType().GetTypeInfo()))
            {
            throw new ArgumentException("DeltaTypeMismatch", nameof(original));
            }

        foreach (PropertyAccessor<TEntity> propertyAccessor in this.GetChangedPropertyNames().Select(s => this._propertiesThatExist[s]))
            {
            propertyAccessor.Copy(this._entity, original);
            }
        }

    /// <summary>
    /// Copies the unchanged property values from the underlying entity (accessible via Delta.GetEntity) to the <paramref name="original" /> entity.
    /// </summary>
    /// <param name="original">The entity to be updated.</param>
    public void CopyUnchangedValues(TEntity original)
        {
        if (!this._entityType.GetTypeInfo().IsAssignableFrom(original.GetType().GetTypeInfo()))
            {
            throw new ArgumentException("DeltaTypeMismatch", nameof(original));
            }

        foreach (PropertyAccessor<TEntity> propertyAccessor in this.GetUnchangedPropertyNames().Select(s => this._propertiesThatExist[s]))
            {
            propertyAccessor.Copy(this._entity, original);
            }
        }

    /// <summary>
    /// Overwrites the <paramref name="original" /> entity with the changes tracked by this Delta.
    /// </summary>
    /// <param name="original">The entity to be updated.</param>
    public void Patch(TEntity original) => this.CopyChangedValues(original);

    /// <summary>
    /// Overwrites the <paramref name="original" /> entity with the values stored in this Delta.
    /// </summary>
    /// <param name="original">The entity to be updated.</param>
    public void Put(TEntity original)
        {
        this.CopyChangedValues(original);
        this.CopyUnchangedValues(original);
        }

    private Dictionary<string, PropertyAccessor<TEntity>> InitializePropertiesThatExist() => Delta<TEntity>._propertyCache.GetOrAdd(this._entityType,
            backingType =>
                backingType
                    .GetTypeInfo()
                    .DeclaredProperties
                        .Where(p => p.SetMethod != null && p.GetMethod != null)
                        .Select(p => new CompiledPropertyAccessor<TEntity>(p))
                        .ToDictionary((PropertyAccessor<TEntity> p) => p.Property.Name));
    }

/// <summary>
/// Represents a <see cref="Delta" /> that can be used when a backing CLR type exists for the entity type whose changes are tracked.
/// </summary>
public abstract class TypedDelta : Delta
    {
    /// <summary>
    /// Gets the actual type of the entity for which the changes are tracked.
    /// </summary>
    public abstract Type EntityType { get; }

    /// <summary>
    /// Gets the expected type of the entity for which the changes are tracked.
    /// </summary>
    public abstract Type ExpectedClrType { get; }
    }

/// <summary>
/// A class the tracks changes (i.e. the Delta) for an entity.
/// </summary>
public abstract class Delta : DynamicObject
    {
    /// <summary>
    /// Clears the Delta and resets the underlying Entity.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Attempts to set the Property called <paramref name="name" /> to the <paramref name="value" /> specified.
    /// </summary>
    /// <returns>True if successful</returns>
    /// <param name="name">The name of the Property</param>
    /// <param name="value">The new value of the Property</param>
    public abstract bool TrySetPropertyValue(string name, object? value);

    /// <summary>
    /// Attempts to get the value of the Property called <paramref name="name" /> from the underlying Entity.
    /// </summary>
    /// <returns>True if the Property was found</returns>
    /// <param name="name">The name of the Property</param>
    /// <param name="value">The value of the Property</param>
    public abstract bool TryGetPropertyValue(string name, [MaybeNullWhen(returnValue: false)] out object? value);

    /// <summary>
    /// Attempts to get the <see cref="T:System.Type" /> of the Property called <paramref name="name" /> from the underlying Entity.
    /// </summary>
    /// <returns>Returns true if the Property was found and false if not.</returns>
    /// <param name="name">The name of the Property</param>
    /// <param name="type">The type of the Property</param>
    public abstract bool TryGetPropertyType(string name, [MaybeNullWhen(returnValue: false)] out Type? type);

    /// <summary>
    /// Overrides the DynamicObject TrySetMember method, so that only the properties of Entity can be set.
    /// </summary>
    public override bool TrySetMember(SetMemberBinder binder, object? value) => this.TrySetPropertyValue(binder.Name, value);

    /// <summary>
    /// Overrides the DynamicObject TryGetMember method, so that only the properties of Entity can be got.
    /// </summary>
    public override bool TryGetMember(GetMemberBinder binder, [MaybeNullWhen(returnValue: false)] out object? result) => this.TryGetPropertyValue(binder.Name, out result);

    /// <summary>
    /// Returns the Properties that have been modified through this Delta as an enumeration of Property Names.
    /// </summary>
    public abstract IEnumerable<string> GetChangedPropertyNames();

    /// <summary>
    /// Returns the Properties that have not been modified through this Delta as an enumeration of Property Names.
    /// </summary>
    public abstract IEnumerable<string> GetUnchangedPropertyNames();

    /// <summary>
    /// Setea las propiedades contenidas en el diccionario.
    /// </summary>
    /// <remarks>
    /// Si alguna propiedad no pudo setearse, lanza una AggregateException con una InvalidOperationException
    /// por cada propiedad que no pudo setearse. El resto de las propiedades son seteadas.
    /// </remarks>
    /// <param name="propertiesValues">Contenedor de las propiedades y valores a setear, puede ser un ExpandoObject.</param>
    public void SetPropertiesValues(IDictionary<string, object> propertiesValues) => new ExceptionAggregator()
            .TryForEach(propertiesValues, propertyValue => this.SetPropertyValue(propertyValue.Key, propertyValue.Value))
            .ThrowIfExceptionsCatched();

    /// <summary>
    /// Setea una propiedad.
    /// </summary>
    /// <exception cref="InvalidOperationException">La propiedad no pudo setearse.</exception>
    public void SetPropertyValue(string name, object value)
        {
        if(!this.TrySetPropertyValue(name, value))
            {
            throw new InvalidOperationException($"Property {name} cannot be set to value {value}");
            }
        }

    /// <summary>
    /// Obtiene el valor de una propiedad.
    /// </summary>
    /// <exception cref="InvalidOperationException">La propiedad no pudo obtenerse.</exception>
    public object GetPropertyValue(string name)
        => this.TryGetPropertyValue(name, out object? propertyValue)
            ? propertyValue!
            : throw new InvalidOperationException($"Property {name} not found.");

    /// <summary>
    /// Retorna un ExpandoObject con la propiedades que fueron seteadas en el delta.
    /// </summary>
    public ExpandoObject ToExpando()
        => this.GetChangedPropertyNames().ToExpando(propertyName => propertyName, this.GetPropertyValue);
    }
