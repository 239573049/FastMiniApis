using System.ComponentModel;
using System.Reflection;

namespace FastMiniApis.Core.Toolkit;

public static class ReflectionUtil
{
    public static string GetClassDescription(this Type type)
    {
        var attributes = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
        {
            var descriptionAttribute = attributes[0] as DescriptionAttribute;
            return descriptionAttribute?.Description;
        }

        return null;
    }

    public static TAttribute? GetAttributeOfType<TAttribute>(this Type type) where TAttribute : Attribute
    {
        var attributes = type.GetCustomAttributes(typeof(TAttribute), false);
        if (attributes.Length > 0)
        {
            return attributes[0] as TAttribute;
        }

        return null;
    }
    
    public static string GetMethodReturnTypeDescription(MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;
        return returnType.GetClassDescription();
    }
}