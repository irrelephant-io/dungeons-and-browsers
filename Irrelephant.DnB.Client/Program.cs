using System.Threading.Tasks;
using Irrelephant.DnB.Client.Infrastructure;
using Irrelephant.DnB.DataTransfer.Extensions;
using Irrelephant.DnB.DataTransfer.Options;
using Irrelephant.DnB.DataTransfer.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorComponentsPreview;

namespace Irrelephant.DnB.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddRazorComponentsRuntimeCompilation();
            ConfigureOptions(builder);
            builder.Services.AddOidcAuthentication(options => {
                builder.Configuration.Bind("OpenIdConnect", options.ProviderOptions);
            });
            builder.Services.AddDungeonsClients();
            RegisterServices(builder.Services);
            await builder.Build().RunAsync();
        }

        private static void ConfigureOptions(WebAssemblyHostBuilder builder)
        {
            builder.Services
                .AddOptions()
                .Configure<ApiConnectionOptions>(opts => builder.Configuration.GetSection("Api").Bind(opts));
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services
                .AddSingleton<IApiTokenProvider, ApiTokenProvider>()
                .AddScoped<IRemoteCombatListenerFactory, RemoteCombatListenerFactory>();
        }
    }
}
