using System.ComponentModel;
using System.Reflection;
using FastMiniApis.Core;
using FastMiniApis.Core.Toolkit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastMiniApis;

public abstract class ServiceBase : IServiceApi
{
    private WebApplication? _webApplication;

    public WebApplication App => _webApplication ?? FastApp.WebApplication;

    public string BaseUri { get; init; }

    public ServiceRouteOptions RouteOptions { get; } = new();

    public string? ServiceName { get; init; }

    /// <summary>
    /// Based on the RouteHandlerBuilder extension, it is used to extend the mapping method, such as
    /// RouteHandlerBuilder = routeHandlerBuilder =>
    /// {
    ///     routeHandlerBuilder.RequireAuthorization("AtLeast21");
    /// };
    /// </summary>
    public Action<RouteHandlerBuilder>? RouteHandlerBuilder { get; init; }


    private bool? _enableProperty;

    protected ServiceBase()
    {
    }

    protected ServiceBase(string baseUri)
    {
        BaseUri = baseUri;
    }

    internal void AutoMapRoute(FastMiniApisOptions globalOptions, PluralizationService pluralizationService)
    {
        var type = GetType();

        var methodInfos = GetMethodsByAutoMapRoute(type, globalOptions);

        var filter = type.GetCustomAttribute<FilterAttribute>();
        var authorize = type.GetCustomAttribute<AuthorizeAttribute>();

        foreach (var method in methodInfos)
        {
            var handler = ServiceBaseUitl.CreateDelegate(method, this);

            string? pattern = null;
            string? httpMethod = null;
            string? methodName = null;
            string? description = null;
            var attribute = method.GetCustomAttribute<RoutePatternAttribute>();
            if (attribute != null)
            {
                httpMethod = attribute.HttpMethod;
                if (attribute.StartWithBaseUri)
                    methodName = attribute.Pattern;
                else
                    pattern = attribute.Pattern;
            }

            description = method.GetCustomAttribute<DescriptionAttribute>()?.Description;

            string prefix = string.Empty;

            if (string.IsNullOrWhiteSpace(httpMethod) || string.IsNullOrWhiteSpace(pattern))
            {
                var result = ParseMethod(globalOptions, method.Name);
                httpMethod ??= result.HttpMethod;
                prefix = result.Prefix;
            }

            pattern ??= ServiceBaseUitl.CombineUris(GetBaseUri(globalOptions, pluralizationService),
                methodName ?? GetMethodName(method, prefix, globalOptions));
            var routeHandlerBuilder = MapMethods(globalOptions, pattern, httpMethod, handler, description);

            var methodFilter = type.GetCustomAttribute<FilterAttribute>();

            if (authorize != null)
            {
                routeHandlerBuilder.RequireAuthorization();
            }

            if (filter != null)
            {
                AddEndpointFilter(routeHandlerBuilder, filter);
            }

            if (methodFilter != null)
            {
                AddEndpointFilter(routeHandlerBuilder, methodFilter);
            }


            RouteHandlerBuilder?.Invoke(routeHandlerBuilder);
        }
    }

    protected virtual void AddEndpointFilter(RouteHandlerBuilder routeHandlerBuilder, FilterAttribute filter)
    {
        foreach (var filterType in filter.FilterTypes)
        {
            if (Activator.CreateInstance(filterType) is IEndpointFilter filterInstance)
            {
                routeHandlerBuilder.AddEndpointFilter(filterInstance);
            }
        }
    }

    protected virtual List<MethodInfo> GetMethodsByAutoMapRoute(Type type, FastMiniApisOptions globalOptions)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var methodInfos = type
            .GetMethods(BindingFlags.DeclaredOnly | bindingFlags)
            .Where(methodInfo =>
                methodInfo.CustomAttributes.All(attr => attr.AttributeType != typeof(IgnoreRouteAttribute)))
            .Concat(type.GetMethods(bindingFlags)
                .Where(methodInfo =>
                    methodInfo.CustomAttributes.Any(attr => attr.AttributeType == typeof(RoutePatternAttribute))))
            .Distinct();

        return methodInfos.Where(methodInfo
            => !methodInfo.IsSpecialName || (methodInfo.IsSpecialName && methodInfo.Name.StartsWith("get_"))).ToList();
    }

    protected virtual string GetBaseUri(ServiceRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        if (!string.IsNullOrWhiteSpace(BaseUri))
            return BaseUri;

        var list = new List<string>()
        {
            RouteOptions.Prefix ?? globalOptions.Prefix ?? string.Empty,
            RouteOptions.Version ?? globalOptions.Version ?? string.Empty,
            ServiceName ??
            GetServiceName(RouteOptions.PluralizeServiceName ?? globalOptions.PluralizeServiceName ?? false
                ? pluralizationService
                : null)
        };

        return string.Join('/', list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(u => u.Trim('/')));
    }

    protected virtual RouteHandlerBuilder MapMethods(ServiceRouteOptions globalOptions, string pattern,
        string? httpMethod,
        Delegate handler, string? description)
    {
        if (!string.IsNullOrWhiteSpace(httpMethod))
            return App.MapMethods(pattern, new[]
                {
                    httpMethod
                }, handler)
                .Description(description)
                .WithOpenApi();

        var httpMethods = GetDefaultHttpMethods(globalOptions);
        if (httpMethods.Length > 0)
            return App.MapMethods(pattern, httpMethods, handler)
                .Description(description)
                .WithOpenApi();

        return App.Map(pattern, handler)
            .Description(description)
            .WithOpenApi();
    }

    protected virtual string[] GetDefaultHttpMethods(ServiceRouteOptions globalOptions)
    {
        if (RouteOptions.MapHttpMethodsForUnmatched.Length > 0)
            return RouteOptions.MapHttpMethodsForUnmatched;

        if (globalOptions.MapHttpMethodsForUnmatched.Length > 0)
            return globalOptions.MapHttpMethodsForUnmatched;

        return Array.Empty<string>();
    }

    protected virtual string GetServiceName(PluralizationService? pluralizationService)
    {
        var serviceName = GetType().Name.TrimEnd("Service", StringComparison.OrdinalIgnoreCase);
        if (pluralizationService == null)
            return serviceName;

        return pluralizationService.Pluralize(serviceName);
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected virtual string GetMethodName(MethodInfo methodInfo, string prefix, ServiceRouteOptions globalOptions)
    {
        var methodName = TrimMethodPrefix(methodInfo.Name);
        if (!(RouteOptions.AutoAppendId ?? globalOptions.AutoAppendId ?? false))
            return ServiceBaseUitl.TrimEndMethodName(methodName);

        var idParameter = methodInfo.GetParameters().FirstOrDefault(p =>
            p.Name!.Equals("id", StringComparison.OrdinalIgnoreCase) &&
            p.GetCustomAttribute<FromBodyAttribute>() == null &&
            p.GetCustomAttribute<FromFormAttribute>() == null &&
            p.GetCustomAttribute<FromHeaderAttribute>() == null &&
            p.GetCustomAttribute<FromQueryAttribute>() == null &&
            p.GetCustomAttribute<FromServicesAttribute>() == null);
        if (idParameter != null)
        {
            var id = idParameter.ParameterType.IsNullableType() || idParameter.HasDefaultValue ? "{id?}" : "{id}";
            return $"{ServiceBaseUitl.TrimEndMethodName(methodName)}/{id}";
        }

        return ServiceBaseUitl.TrimEndMethodName(methodName);

        string TrimMethodPrefix(string name)
        {
            if (RouteOptions.DisableTrimMethodPrefix ?? globalOptions.DisableTrimMethodPrefix ?? false)
                return name;

            return name.Substring(prefix.Length);
        }
    }

    protected virtual (string? HttpMethod, string Prefix) ParseMethod(ServiceRouteOptions globalOptions,
        string methodName)
    {
        var getPrefixes = RouteOptions.GetPrefixes ?? globalOptions.GetPrefixes!;
        if (_enableProperty == true)
        {
            getPrefixes.Insert(0, "get_");
        }

        var prefix = ServiceBaseUitl.ParseMethodPrefix(getPrefixes, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("GET", prefix);

        prefix = ServiceBaseUitl.ParseMethodPrefix(RouteOptions.PostPrefixes ?? globalOptions.PostPrefixes!,
            methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("POST", prefix);

        prefix = ServiceBaseUitl.ParseMethodPrefix(RouteOptions.PutPrefixes ?? globalOptions.PutPrefixes!, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("PUT", prefix);

        prefix = ServiceBaseUitl.ParseMethodPrefix(RouteOptions.DeletePrefixes ?? globalOptions.DeletePrefixes!,
            methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("DELETE", prefix);

        return (null, string.Empty);
    }
}