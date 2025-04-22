using Core.Localization.Abstractions;
using Core.Localization.Configuration;
using Core.Localization.Providers;
using Core.Localization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Localization.Extensions;

/// <summary>
/// Extension methods for registering localization services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Core.Localization services to the service collection
    /// </summary>
    public static IServiceCollection AddCoreLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions>? configureOptions = null)
    {
        // Register default options
        services.AddOptions<LocalizationOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection("Localization").Bind(options);
                configureOptions?.Invoke(options);
            });

        // Register core services
        services.AddMemoryCache();
        services.AddLogging();
        
        // Register providers
        services.AddSingleton<IResourceProvider, ResxResourceProvider>();
        services.AddSingleton<IResourceProvider, JsonResourceProvider>();
        services.AddSingleton<IResourceProvider, YamlResourceProvider>();
        
        // Register main services
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IFormatterService, FormatterService>();
        
        return services;
    }

    /// <summary>
    /// Adds a custom resource provider
    /// </summary>
    public static IServiceCollection AddResourceProvider<TProvider>(
        this IServiceCollection services)
        where TProvider : class, IResourceProvider
    {
        services.AddSingleton<IResourceProvider, TProvider>();
        return services;
    }

    /// <summary>
    /// Adds only JSON resource provider
    /// </summary>
    public static IServiceCollection AddJsonResourceProvider(this IServiceCollection services)
    {
        services.RemoveAll<IResourceProvider>();
        services.AddSingleton<IResourceProvider, JsonResourceProvider>();
        return services;
    }

    /// <summary>
    /// Adds only YAML resource provider
    /// </summary>
    public static IServiceCollection AddYamlResourceProvider(this IServiceCollection services)
    {
        services.RemoveAll<IResourceProvider>();
        services.AddSingleton<IResourceProvider, YamlResourceProvider>();
        return services;
    }

    /// <summary>
    /// Adds only RESX resource provider
    /// </summary>
    public static IServiceCollection AddResxResourceProvider(this IServiceCollection services)
    {
        services.RemoveAll<IResourceProvider>();
        services.AddSingleton<IResourceProvider, ResxResourceProvider>();
        return services;
    }

    /// <summary>
    /// Configures localization options
    /// </summary>
    public static IServiceCollection ConfigureLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }

    /// <summary>
    /// Adds in-memory resource provider for testing
    /// </summary>
    public static IServiceCollection AddInMemoryResourceProvider(
        this IServiceCollection services,
        IDictionary<string, IDictionary<string, string>> resources)
    {
        services.AddSingleton<IResourceProvider>(
            new InMemoryResourceProvider(resources));
        return services;
    }
}
