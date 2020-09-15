using System.Net.Http;
using System.Threading.Tasks;
using Irrelephant.DnB.Server.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Irrelephant.DnB.Server.Tests.Fixtures
{
    public class ServerFixture : IAsyncLifetime
    {
        private TestServer _testServer;

        public HubConnection CombatConnection;

        public HttpClient HttpClient { get; private set; }

        public async Task InitializeAsync()
        {
            _testServer = new TestServer(new WebHostBuilder()
                .ConfigureServices(services => {
                    services.AddScoped<ICombatRepository, MemoryCombatRepository>();
                })
                .UseStartup<Startup>());
            HttpClient = _testServer.CreateClient();
            CombatConnection = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/combat", 
                    cfg => {
                        cfg.HttpMessageHandlerFactory = (_ => _testServer.CreateHandler());
                        cfg.Transports = HttpTransportType.LongPolling;
                    })
                .Build();
            await CombatConnection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await CombatConnection.DisposeAsync();
            _testServer.Dispose();
        }
    }
}
