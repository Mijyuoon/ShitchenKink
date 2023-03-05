using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;
using Polly.Extensions.Http;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Extensions;
using ShitchenKink.Core.Interfaces;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Core;

public static class Setup
{
    private const string AuthConfigPath = "Bot:Auth";
    private const string SocketConfigPath = "Bot:Socket";

    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Required Discord configuration
        services.AddConfiguration<DiscordAuthConfig>(configuration, AuthConfigPath);
        services.AddConfiguration<DiscordSocketConfig>(configuration, SocketConfigPath);

        // Discord.Net services
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();

        // Application services
        services.AddSingleton<BotService>();
        services.AddSingleton<BotCommandService>();
        services.AddSingleton<DispatchService>();
        services.AddSingleton<HttpService>();

        // HTTP service client and its configuration
        services.AddConfiguration<HttpClientConfig>(configuration, HttpClientConfig.Path);
        services.AddHttpServiceClient();

        // Hosted application services (in startup order)
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<BotService>());

        // Message event handlers (in call order)
        services.AddSingleton<IMessageHandler>(provider => provider.GetRequiredService<BotCommandService>());
    }

    public static async Task UseCoreServices(this IServiceProvider services)
    {
        // Nothing to do here
        await Task.CompletedTask;
    }

    private static void AddHttpServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<HttpService>()
            .AddPolicyHandler((sp, _) =>
            {
                var config = sp.GetRequiredService<HttpClientConfig>();
                return HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(config.RetryCount, _ => config.RetryWait);
            })
            .AddPolicyHandler((sp, _) =>
            {
                var config = sp.GetRequiredService<HttpClientConfig>();
                return Policy.TimeoutAsync<HttpResponseMessage>(config.Timeout);
            });
    }
}