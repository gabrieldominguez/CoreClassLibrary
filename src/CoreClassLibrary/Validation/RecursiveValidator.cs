#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Objects;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CoreClassLibrary.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class SkipRecursiveValidation : Attribute;

/// <summary>
/// The helper will recursively traverse your object graph and invoke validation against DataAnnotations.
/// </summary>
/// <see cref="https://github.com/reustmd/DataAnnotationsValidatorRecursive/blob/master/DataAnnotationsValidator/DataAnnotationsValidator/DataAnnotationsValidator.cs"/>
public static class RecursiveValidator
    {
    public static void RecursiveValidate(this object instance)
        => instance.TryRecursiveValidate().ThrowIfAny();

    public static ICollection<ValidationResult> TryRecursiveValidate(this object instance)
        => RecursiveValidator.TryRecursiveValidate(instance, []);

    private static ICollection<ValidationResult> TryRecursiveValidate(object instance, HashSet<object> validatedObjects)
        {
        //short-circuit to avoid infinit loops on cyclical object graphs
        if (validatedObjects.Contains(instance))
            {
            return [];
            }

        validatedObjects.Add(instance);
        ICollection<ValidationResult> validationResults = instance.TryValidate();

        List<PropertyInfo> properties = instance.GetType().GetProperties().Where(prop => prop.CanRead
                && prop.GetCustomAttributes(typeof(SkipRecursiveValidation), false).Length == 0
                && prop.GetIndexParameters().Length == 0).ToList();

        foreach (PropertyInfo property in properties)
            {
            if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

            object? value = instance.GetPropertyValue(property.Name);

            if (value == null) continue;

            if (value is IEnumerable asEnumerable)
                {
                foreach (object? enumObj in asEnumerable)
                    {
                    if (enumObj != null)
                        {
                        ICollection<ValidationResult> nestedResults = RecursiveValidator.TryRecursiveValidate(enumObj, validatedObjects);
                        foreach (ValidationResult validationResult in nestedResults)
                            {
                            validationResults.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property.Name + '.' + x)));
                            }
                        };
                    }
                }
            else
                {
                ICollection<ValidationResult> nestedResults = RecursiveValidator.TryRecursiveValidate(value, validatedObjects);
                foreach (ValidationResult validationResult in nestedResults)
                    {
                    validationResults.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property.Name + '.' + x)));
                    }
                }
            }

        return validationResults;
        }
    }
