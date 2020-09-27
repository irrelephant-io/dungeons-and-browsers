using System;
using Irrelephant.DnB.DataTransfer.Clients;
using Irrelephant.DnB.DataTransfer.Infrastructure;
using Irrelephant.DnB.DataTransfer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Irrelephant.DnB.DataTransfer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDungeonsClients(this IServiceCollection services)
        {
            services.AddHttpClient<IAuthHttpClient, AuthHttpClient>((serviceProvider, client) => {
                var connection = serviceProvider.GetRequiredService<IOptionsMonitor<ApiConnectionOptions>>();
                client.BaseAddress = new Uri(connection.CurrentValue.BaseAddress, "/api");
            });
            services.AddAuthenticatedHttpClient<ILobbyHttpClient, LobbyHttpClient>();
            services.AddTransient<ApiAuthorizationMessageHandler>();
            return services;
        }

        private static void AddAuthenticatedHttpClient<TClient, TImplementation>(this IServiceCollection services)
            where TClient : class
            where TImplementation : class, TClient
        {
            services
                .AddHttpClient<TClient, TImplementation>((serviceProvider, client) => {
                    var connection = serviceProvider.GetRequiredService<IOptionsMonitor<ApiConnectionOptions>>();
                    client.BaseAddress = new Uri(connection.CurrentValue.BaseAddress, "/api");
                })
                .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();
        }
    }
}
