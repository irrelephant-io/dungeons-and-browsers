using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Utils;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.DataTransfer.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace Irrelephant.DnB.Client.Networking
{
    public class RemoteCombatListener : IRemoteCombatListener, IAsyncDisposable
    {
        private readonly HubConnection _connection;

        private Guid CombatId { get; }

        public RemoteCombatListener(IApiTokenProvider apiTokenProvider, Uri baseAddress, Guid combatId)
        {
            CombatId = combatId;
            _connection = new HubConnectionBuilder()
                .WithUrl(new Uri(baseAddress, "combat"), opts => {
                    opts.AccessTokenProvider = apiTokenProvider.GetToken;
                })
                .Build();
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            _connection.On<CombatSnapshot>("Joined", snapshot => OnJoinedCombat?.Invoke(snapshot));
            _connection.On<CharacterSnapshot>("CharacterUpdated", snapshot => OnCharacterUpdated?.Invoke(snapshot));
            _connection.On("MyTurn", () => OnMyTurn?.Invoke());
            _connection.On<Guid>("DrawCard", cardId => OnDrawCard?.Invoke(cardId));
            _connection.On<Guid>("DiscardCard", cardId => OnDiscardCard?.Invoke(cardId));
            _connection.On<Guid>("CardPlayed", cardId => OnCardPlayed?.Invoke(cardId));
            _connection.On("ReshuffleDiscardPile", () => OnReshuffleDiscardPile?.Invoke());
            _connection.On<Guid>("LeftCombat", (characterId) => LeftCombat?.Invoke(characterId));
            _connection.On<JoinFightMessage>("CharacterJoined", message => OnCharacterJoined?.Invoke(message));
            _connection.On<JoinFightMessage>("PendingCombat", message => OnPendingCombat?.Invoke(message));
        }

        public event Action<CombatSnapshot> OnJoinedCombat;

        public event Action<CharacterSnapshot> OnCharacterUpdated;

        public event Action<Guid> OnDrawCard;

        public event Action<Guid> OnDiscardCard;

        public event Action<Guid> OnCardPlayed;

        public event Action OnReshuffleDiscardPile;

        public event Action OnMyTurn;

        public event Action<Guid> LeftCombat;

        public event Action<JoinFightMessage> OnCharacterJoined;

        public event Action<JoinFightMessage> OnPendingCombat;

        public async Task NotifyJoinedAsync()
        {
            await _connection.SendAsync("JoinCombat", CombatId);
        }

        public async Task<bool> NotifyEndTurnAsync()
        {
            await _connection.SendAsync("EndTurn", CombatId);
            return true;
        }

        public async Task PlayCard(CardTargets targets)
        {
            var targetIds = targets.EffectTargets.Select(kvp => kvp.Key.ArrayOf().Union(kvp.Value).ToArray()).ToArray();
            await _connection.SendAsync("PlayCard", CombatId, targets.CardId, targetIds);
        }

        public async Task StartAsync()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Uh oh!" + e.Message);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
