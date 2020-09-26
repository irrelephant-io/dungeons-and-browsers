using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Server.Networking;
using Irrelephant.DnB.Server.Repositories;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public async Task JoinCombat(Guid combatId)
        {
            var combat = await GetOrCreateCombat(combatId);
            var joiningPlayer = TryFindJoiningPlayer(combat);
            if (joiningPlayer != null)
            {
                await RejoinExistingPlayer(joiningPlayer, combat);
            }
            else
            {
                await JoinNewPlayer(combat);
                StartCombatIfNecessary(combat);
            }
        }

        private async Task JoinNewPlayer(Combat combat)
        {
            var joiningCharacter = CombatFactory.SetupPlayer(_services);
            joiningCharacter.Name = Context.User.Identity.Name;
            joiningCharacter.ConnectionId = Context.ConnectionId;
            var player = new RemotePlayerCharacterController(joiningCharacter)
            {
                ControllingIdentity = Context.User.Identity
            };
            if (combat.IsStarted)
            {
                await combat.AddDefender(0, player);
            }
            else
            {
                await combat.AddAttacker(0, player);
            }
            var snapshot = combat.GetSnapshot();
            snapshot.ActiveCharacterId = joiningCharacter.Id;
            await Clients.Caller.Joined(snapshot);
        }

        private async Task RejoinExistingPlayer(RemotePlayerCharacterController joiningPlayer, Combat combat)
        {
            joiningPlayer.RemoteCharacter.ConnectionId = Context.ConnectionId;
            var snapshot = combat.GetSnapshot();
            snapshot.ActiveCharacterId = joiningPlayer.RemoteCharacter.Id;
            await Clients.Caller.Joined(snapshot);
        }

        private RemotePlayerCharacterController TryFindJoiningPlayer(Combat combat)
        {
            return combat.Combatants.Union(combat.PendingCombatants).FirstOrDefault(cc =>
                cc is RemotePlayerCharacterController remoteCc
                && remoteCc.ControllingIdentity.Name == Context.User.Identity.Name
            ) as RemotePlayerCharacterController;
        }

        private async Task<Combat> GetOrCreateCombat(Guid? combatId)
        {
            Combat combat;

            if (combatId.HasValue)
            {
                combat = await _combatRepo.GetCombat(combatId.Value);
                if (combat != null)
                {
                    return combat;
                }
            }

            combat = CombatFactory.BuildCombat(_services);
            combat.CombatId = combatId ?? Guid.NewGuid();
            await _combatRepo.AddCombat(combat);
            return combat;
        }

        private void StartCombatIfNecessary(Combat combat)
        {
            if (combat.IsStarted)
            {
                return;
            }

            new Thread(async () => {
                await combat.RunCombat();
                await EndCombat(combat);
            }).Start();
        }

        private async Task EndCombat(Combat combat)
        {
            await _combatRepo.RemoveCombat(combat);
        }

        [Authorize]
        public async Task PlayCard(Guid combatId, Guid cardId, Guid[][] targets)
        {
            var combat = await _combatRepo.GetCombat(combatId);
            if (combat.CurrentActiveCharacter is RemotePlayerCharacterController remoteCharacterController
                && remoteCharacterController.RemoteCharacter.ConnectionId == Context.ConnectionId)
            {
                var card = remoteCharacterController.RemoteCharacter.Hand.First(it => it.Id == cardId);
                var effectTargets = targets.ToDictionary(t => t.First(), t => t.Skip(1).ToArray());
                await card.Play(remoteCharacterController.RemoteCharacter, new RemoteTargetProvider(combat, effectTargets));
            }
            else
            {
                throw new NotMyTurnException();
            }
        }

        [Authorize]
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
