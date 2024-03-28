﻿
using System.Reflection;

namespace System;

public static class TypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static bool IsNullableType(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// Determine if a class is derived from a generic class
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType">base class type, Only interface generics are supported</param>
    /// <returns></returns>
    public static bool IsImplementerOfGeneric(this Type type, Type genericType)
    {
        if (!genericType.GetTypeInfo().IsGenericType)
            return false;

        if (genericType.IsInterface)
            return type.IsImplementerOfGenericInterface(genericType);

        return type.IsImplementerOfGenericClass(genericType);
    }

    private static bool IsImplementerOfGenericInterface(this Type type, Type genericType)
    {
        return type.GetInterfaces().Any(interfaceType =>
        {
            var current = interfaceType.GetTypeInfo().IsGenericType ?
                interfaceType.GetGenericTypeDefinition() : interfaceType;
            return current == genericType;
        });
    }

    private static bool IsImplementerOfGenericClass(this Type type, Type genericType)
    {
        var currentType = type.GetTypeInfo().IsGenericType ?
            type.GetGenericTypeDefinition() : type;
        if (currentType == genericType)
            return true;

        var baseType = currentType.BaseType;
        if (baseType == null)
            return false;

        return baseType.IsImplementerOfGenericClass(genericType);
    }

    public static object? GetDefaultValue(this Type type)
        => type.IsValueType ? Activator.CreateInstance(type) : null;

    /// <summary>
    /// is an explicit class
    /// non-generic class, non-interface class
    /// </summary>
    /// <returns></returns>
    public static bool IsConcrete(this Type type, bool isContainAbstract = false)
    {
        if (isContainAbstract)
            return type is { IsGenericTypeDefinition: false, IsInterface: false };

        return type is { IsGenericTypeDefinition: false, IsInterface: false, IsAbstract: false };
    }
}
