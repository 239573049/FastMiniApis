﻿using System.Reflection;

namespace FastMiniApis.Core;

public static class FastApp
{
    private static IServiceProvider? _rootServiceProvider;

    public static IServiceProvider RootServiceProvider
    {
        get
        {
            if (_rootServiceProvider == null) Build();
            return _rootServiceProvider!;
        }
        private set => _rootServiceProvider = value;
    }

    public static WebApplication WebApplication;

    private static IServiceCollection Services { get; set; } = new ServiceCollection();

    private static IEnumerable<Assembly>? Assemblies { get; set; }

    public static void Build() => Build(Services.BuildServiceProvider());

    public static void Build(WebApplication application)
    {
        WebApplication = application;
        Build(application.Services);
    }

    public static void Build(IServiceProvider serviceProvider) => RootServiceProvider = serviceProvider;

    public static TService? GetService<TService>()
        => GetService<TService>(RootServiceProvider);

    public static TService? GetService<TService>(IServiceProvider serviceProvider)
        => serviceProvider.GetService<TService>();

    public static TService GetRequiredService<TService>() where TService : notnull
        => GetRequiredService<TService>(RootServiceProvider);

    public static TService GetRequiredService<TService>(IServiceProvider serviceProvider) where TService : notnull
        => serviceProvider.GetRequiredService<TService>();

    public static void TrySetServiceCollection(IServiceCollection services)
    {
        if (Services.Count == 0) SetServiceCollection(services);
    }

    public static void SetServiceCollection(IServiceCollection services)
    {
        Services = services;
        _rootServiceProvider = null;
    }

    public static IServiceCollection GetServices() => Services;

    /// <summary>
    /// Set the global Assembly collection (only if Assembly is not assigned a value)
    /// </summary>
    /// <param name="assemblies"></param>
    public static void TrySetAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        Assemblies ??= assemblies;
    }

    /// <summary>
    /// Set the global Assembly collection (only if Assembly is not assigned a value)
    /// </summary>
    /// <param name="assemblies"></param>
    public static void TrySetAssemblies(IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        Assemblies ??= assemblies.ToArray();
    }

    /// <summary>
    /// Set the global Assembly collection
    /// </summary>
    /// <param name="assemblies"></param>
    public static void SetAssemblies(params Assembly[] assemblies)
        => Assemblies = assemblies;

    /// <summary>
    /// Set the global Assembly collection
    /// </summary>
    /// <param name="assemblies"></param>
    public static void SetAssemblies(IEnumerable<Assembly> assemblies)
        => Assemblies = assemblies;

    public static void TryAddAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        if (Assemblies == null)
            Assemblies = assemblies;
        else
            Assemblies = Assemblies.Concat(assemblies);
    }

    public static void TryAddAssemblies(IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        if (Assemblies == null)
            Assemblies = assemblies.ToArray();
        else
            Assemblies = Assemblies.Concat(assemblies);
    }

    public static IEnumerable<Assembly> GetAssemblies() => Assemblies ?? AppDomain.CurrentDomain.GetAssemblies();
}