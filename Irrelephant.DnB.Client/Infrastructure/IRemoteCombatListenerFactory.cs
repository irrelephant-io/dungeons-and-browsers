using System;
using Irrelephant.DnB.Client.Networking;

namespace Irrelephant.DnB.Client.Infrastructure
{
    public interface IRemoteCombatListenerFactory
    {
        public IRemoteCombatListener MakeListener(Guid combatId);
    }
}
