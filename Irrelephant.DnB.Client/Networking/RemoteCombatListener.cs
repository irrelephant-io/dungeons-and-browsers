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
                .WithUrl("https://localhost:5001/combat")
                .Build();
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            _connection.On<CombatSnapshot>("Joined", snapshot => OnJoinedCombat?.Invoke(snapshot));
            _connection.On<CharacterSnapshot>("CharacterUpdated", snapshot => OnCharacterUpdated?.Invoke(snapshot));
            _connection.On("MyTurn", () => OnMyTurn?.Invoke());
        }

        public event Action<CombatSnapshot> OnJoinedCombat;

        public event Action<CharacterSnapshot> OnCharacterUpdated;

        public event Action OnMyTurn;

        public async Task NotifyJoinedAsync()
        {
            Console.WriteLine("Joining combat!");
            await _connection.SendAsync("JoinCombat");
            Console.WriteLine("Request sent!");
        }

        public async Task<bool> NotifyEndTurnAsync()
        {
            await _connection.SendAsync("EndTurn");
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
