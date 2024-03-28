using System.Linq.Expressions;
using System.Reflection;

namespace FastMiniApis.Core.Toolkit;


public static class ServiceBaseUitl
{
    public static Delegate CreateDelegate(MethodInfo methodInfo, object targetInstance)
    {
        var type = Expression.GetDelegateType(methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType)
            .Concat(new List<Type>
            {
                methodInfo.ReturnType
            }).ToArray());
        return Delegate.CreateDelegate(type, targetInstance, methodInfo);
    }

    public static string CombineUris(params string[] uris) => string.Join("/", uris.Select(u => u.Trim('/')));

    public static string TrimEndMethodName(string methodName)
        => methodName.TrimEnd("Async", StringComparison.OrdinalIgnoreCase);

    public static string ParseMethodPrefix(IEnumerable<string> prefixes, string methodName)
    {
        var newMethodName = methodName;
        var prefix = prefixes.FirstOrDefault(prefix => newMethodName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        if (prefix is not null)
            return methodName.Substring(0, prefix.Length);

        return string.Empty;
    }
}
