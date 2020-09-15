using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Server.Networking;
using Irrelephant.DnB.Server.Repositories;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Hubs
{
    public class CombatHub : Hub<ICombatClient>
    {
        private readonly IServiceProvider _services;
        private readonly ICombatRepository _combatRepo;

        public CombatHub(IServiceProvider services, ICombatRepository combatRepo)
        {
            _services = services;
            _combatRepo = combatRepo;
        }
        
        public async Task JoinCombat()
        {
            var combat = CombatFactory.BuildCombat();
            combat.CombatId = Guid.NewGuid();
            var joiningCharacter = CombatFactory.SetupPlayer(_services);
            joiningCharacter.ConnectionId = Context.ConnectionId;
            var player = new RemotePlayerCharacterController(joiningCharacter);
            await combat.AddAttacker(0, player);
            var snapshot = combat.GetSnapshot();
            snapshot.ActiveCharacterId = player.Character.Id;
            await Clients.All.Joined(snapshot);
            await _combatRepo.AddCombat(combat);
            StartCombatIfNecessary(combat);
        }

        private static void StartCombatIfNecessary(Combat combat)
        {
            if (combat.IsStarted)
            {
                return;
            }

            new Thread(async () => { await combat.RunCombat(); }).Start();
        }

        public async Task EndTurn(Guid combatId)
        {
            var combat = await _combatRepo.GetCombat(combatId);
            if (combat.CurrentActiveCharacter is RemotePlayerCharacterController remoteCharacterController
            && remoteCharacterController.RemoteCharacter.ConnectionId == Context.ConnectionId)
            {
                await remoteCharacterController.EndTurn();
            }
            else
            {
                throw new NotMyTurnException();
            }
        }
    }
}
