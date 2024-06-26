﻿using System.Reflection;
using FastMiniApis.Core;
using Microsoft.Extensions.Options;

namespace FastMiniApis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddService(this IServiceCollection services,
        ServiceType serviceType = ServiceType.Transient, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 扫描程序集中的所有类继承自ServiceBase的类
        var types = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(ServiceBase)));

        GetMethodsByAutoMapRoute(types, services);
        foreach (var type in types)
        {
            switch (serviceType)
            {
                case ServiceType.Singleton:
                    services.AddSingleton(typeof(IServiceApi), type);
                    break;
                case ServiceType.Scoped:
                    services.AddScoped(typeof(IServiceApi), type);
                    break;
                case ServiceType.Transient:
                    services.AddTransient(typeof(IServiceApi), type);
                    break;
            }
        }

        return services;
    }

    /// <summary>
    /// 扫描类和类的所有方法中的特性
    /// </summary>
    private static void GetMethodsByAutoMapRoute(IEnumerable<Type> types, IServiceCollection services)
    {
        var values = new List<Type>();
        foreach (var type in types)
        {
            // 搜索type的特性和里面的所有public的方法中的特性
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var filter = methods.Where(x => x.GetCustomAttribute<FilterAttribute>() != null).SelectMany(x =>
            {
                var type = x.GetCustomAttribute<FilterAttribute>();

                return type!.FilterTypes;
            }).ToList();

            var filterTypes = type.GetCustomAttribute<FilterAttribute>()?.FilterTypes;
            if (filterTypes != null)
                filter.AddRange(filterTypes);

            values.AddRange(filter);
        }

        values = values.Distinct().ToList();

        foreach (var f in values)
        {
            services.AddSingleton(f);
        }
    }

    public static void MapFastMiniApis(this WebApplication webApplication)
    {
        FastApp.Build(webApplication);

        var serviceMapOptions = webApplication.Services.GetRequiredService<IOptions<FastMiniApisOptions>>().Value;

        foreach (var serviceType in webApplication.Services.GetServices(typeof(IServiceApi)))
        {
            var serviceInstance = (ServiceBase)serviceType;
            if (serviceInstance.RouteOptions.DisableAutoMapRoute ?? serviceMapOptions.DisableAutoMapRoute ?? false)
                continue;

            serviceInstance.AutoMapRoute(serviceMapOptions, serviceMapOptions.Pluralization);
        }
    }
}