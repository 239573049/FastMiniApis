using System.Reflection;
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