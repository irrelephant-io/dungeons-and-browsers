using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Networking;

namespace Irrelephant.DnB.Client.Networking
{
    public interface IRemoteCombatListener
    {
        event Action<CombatSnapshot> OnJoinedCombat;

        event Action<CharacterSnapshot> OnCharacterUpdated;

        public Task NotifyJoined();
    }
}
