using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShitchenKink.Core.Extensions;

public static class ConfigurationExtensions
{
    public static T? GetSection<T>(
        this IConfiguration configuration, string section)
        => configuration.GetSection(section).Get<T>();

    public static IServiceCollection AddConfiguration<T>(
        this IServiceCollection services, IConfiguration configuration) where T : class
        => services.AddSingleton(configuration.Get<T>()!);

    public static IServiceCollection AddConfiguration<T>(
        this IServiceCollection services, IConfiguration configuration, string section) where T : class
        => services.AddSingleton(configuration.GetSection(section).Get<T>()!);
}