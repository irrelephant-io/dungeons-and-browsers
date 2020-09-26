using System;
using Irrelephant.DnB.Client.Networking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Irrelephant.DnB.Client.Infrastructure
{
    public class RemoteCombatListenerFactory : IRemoteCombatListenerFactory
    {
        private readonly IServiceProvider _provider;

        public RemoteCombatListenerFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IRemoteCombatListener MakeListener(Guid combatId)
        {
            var tokenProvider = _provider.GetRequiredService<IApiTokenProvider>();
            var baseUrl = _provider.GetRequiredService<IOptionsMonitor<ApiConnectionOptions>>().CurrentValue.BaseAddress;
            return new RemoteCombatListener(tokenProvider, baseUrl, combatId);
        }
    }
}
