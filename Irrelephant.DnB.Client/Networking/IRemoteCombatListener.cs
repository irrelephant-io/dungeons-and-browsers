using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Networking;

namespace Irrelephant.DnB.Client.Networking
{
    public interface IRemoteCombatListener
    {
        event Action<CombatSnapshot> OnJoinedCombat;

        event Action<CharacterSnapshot> OnCharacterUpdated;

        event Action OnMyTurn;

        event Action<Guid> OnDrawCard;

        event Action<Guid> OnDiscardCard;

        event Action OnReshuffleDiscardPile;

        public Task NotifyJoinedAsync();

        public Task<bool> NotifyEndTurnAsync(Guid combatId);

        public Task StartAsync();
    }
}
