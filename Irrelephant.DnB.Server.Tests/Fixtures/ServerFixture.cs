using System;
using System.Net.Http;
using System.Threading.Tasks;
using Irrelephant.DnB.Server.Authentication.Services;
using Irrelephant.DnB.Server.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Irrelephant.DnB.Server.Tests.Fixtures
{
    public class ServerFixture : IAsyncLifetime
    {
        private TestServer _testServer;

        public HubConnection CombatConnection;

        public IServiceProvider Services;

        public HttpClient HttpClient { get; private set; }

        public async Task InitializeAsync()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(cfg => {
                    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .ConfigureTestServices(services => {
                    services.AddSingleton<ITokenValidator, TestTokenValidator>();
                }));
            HttpClient = _testServer.CreateClient();
            CombatConnection = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/combat", 
                    cfg => {
                        cfg.HttpMessageHandlerFactory = (_ => _testServer.CreateHandler());
                        cfg.Transports = HttpTransportType.LongPolling;
                        cfg.AccessTokenProvider = () => TestTokenProvider.GetToken(HttpClient);
                    })
                .Build();
            Services = _testServer.Services;
            await CombatConnection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await CombatConnection.DisposeAsync();
            _testServer.Dispose();
        }
    }
}
