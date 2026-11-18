using System.Dynamic;
using System.Reflection;

namespace CoreClassLibrary.Objects.DLR;

/// <summary>
/// Type wrapper to call static members dynamically.
/// </summary>
/// <see cref="https://blogs.msdn.microsoft.com/davidebb/2009/10/23/using-c-dynamic-to-call-static-members/"/>
public class StaticMembersDynamicWrapper(Type type) : DynamicObject
    {
    // Handle static properties
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
        PropertyInfo? prop = type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
        if (prop == null)
            {
            result = null;
            return false;
            }

        result = prop.GetValue(null, null);
        return true;
        }

    // Handle static methods
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
        //MethodInfo method = this.type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
        MethodInfo? method = type.GetMethod(binder.Name, args?.Select(arg => arg!.GetType()).ToArray() ?? []);

        if (method == null || !method.IsStatic)
            {
            result = null;
            return false;
            }

        result = method.Invoke(null, args);
        return true;
        }
    }
