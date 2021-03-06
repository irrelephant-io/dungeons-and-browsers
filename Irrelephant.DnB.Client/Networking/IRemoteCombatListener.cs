using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.Client.Networking;

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

    event Action<JoinFightMessage> OnCharacterJoined;

    public event Action<JoinFightMessage> OnPendingCombat;

    public Task NotifyJoinedAsync();

    public Task<bool> NotifyEndTurnAsync();

    public Task PlayCard(CardTargets targets);

    public Task StartAsync();
}