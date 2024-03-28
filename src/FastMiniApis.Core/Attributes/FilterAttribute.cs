namespace System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class FilterAttribute : Attribute
{
    public readonly Type[] FilterTypes;

    public FilterAttribute(params Type[] filterTypes)
    {
        FilterTypes = filterTypes;
    }
}