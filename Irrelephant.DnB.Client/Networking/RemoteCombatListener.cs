using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Networking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Irrelephant.DnB.Client.Networking
{
    public class RemoteCombatListener : IRemoteCombatListener, IAsyncDisposable
    {
        private readonly HubConnection _connection;

        public RemoteCombatListener(NavigationManager navigationManager)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44364/combat")
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
            _connection.On("ReshuffleDiscardPile", () => OnReshuffleDiscardPile?.Invoke());
        }

        public event Action<CombatSnapshot> OnJoinedCombat;

        public event Action<CharacterSnapshot> OnCharacterUpdated;

        public event Action<Guid> OnDrawCard;

        public event Action<Guid> OnDiscardCard;

        public event Action OnReshuffleDiscardPile;

        public event Action OnMyTurn;

        public async Task NotifyJoinedAsync()
        {
            await _connection.SendAsync("JoinCombat");
        }

        public async Task<bool> NotifyEndTurnAsync(Guid combatId)
        {
            await _connection.SendAsync("EndTurn", combatId);
            return true;
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

        public ValueTask DisposeAsync()
        {
            return _connection.DisposeAsync();
        }
    }
}
