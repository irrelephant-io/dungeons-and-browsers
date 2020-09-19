using System;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.Client.Networking
{
    public interface IRemoteCombatListener
    {
        event Action<CombatSnapshot> OnJoinedCombat;

        event Action<CharacterSnapshot> OnCharacterUpdated;

        event Action OnMyTurn;

        event Action<Guid> OnDrawCard;

        event Action<Guid> OnDiscardCard;

        event Action<Guid> OnCardPlayed;

        event Action OnReshuffleDiscardPile;

        event Action<Guid> LeftCombat; 

        public Task NotifyJoinedAsync();

        public Task<bool> NotifyEndTurnAsync(Guid combatId);

        public Task PlayCard(Guid combatId, CardTargets targets);

        public Task StartAsync();
    }
}
