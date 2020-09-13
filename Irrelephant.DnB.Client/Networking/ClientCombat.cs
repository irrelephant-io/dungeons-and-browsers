using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Networking;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientCombat : Combat
    {
        public bool MyTurn { get; private set; }

        public bool IsReady { get; private set; }

        private readonly IRemoteCombatListener _remoteCombatListener;

        public ClientCombat(IRemoteCombatListener remoteCombatListener)
        {
            _remoteCombatListener = remoteCombatListener;
            _remoteCombatListener.NotifyJoinedAsync();
            _remoteCombatListener.OnJoinedCombat += OnJoinedCombat;
            _remoteCombatListener.OnCharacterUpdated += OnCharacterUpdated;
            _remoteCombatListener.OnMyTurn += OnMyTurn;
        }

        public async Task EndTurn()
        {
            await _remoteCombatListener.NotifyEndTurnAsync();
            NotifyUpdate();
        }

        private void OnMyTurn()
        {
            MyTurn = true;
            NotifyUpdate();
        }

        private void OnJoinedCombat(CombatSnapshot snapshot)
        {
            Console.WriteLine("Got snapshot!");
            Attackers = snapshot.Attackers.Select(c => new RemoteCharacterController(MapCharacter(c))).ToArray();
            Defenders = snapshot.Defenders.Select(c => new RemoteCharacterController(MapCharacter(c))).ToArray();
            Console.WriteLine(Attackers.Count());
            IsReady = true;
            NotifyUpdate();
        }

        private void OnCharacterUpdated(CharacterSnapshot snapshot)
        {
            var characterToUpdate = FindCharacterById(snapshot.Id);
            characterToUpdate.Health = snapshot.Health;
            characterToUpdate.MaxHealth = snapshot.MaxHealth;
            characterToUpdate.GraphicId = snapshot.GraphicId;
            characterToUpdate.Name = snapshot.Name;
            NotifyUpdate();
        }

        private Character MapCharacter(CharacterSnapshot snapshot)
        {
            return new NonPlayerCharacter {
                Id = snapshot.Id,
                Name = snapshot.Name,
                Health = snapshot.Health,
                MaxHealth = snapshot.MaxHealth,
                GraphicId = snapshot.GraphicId
            };
        }
    }
}
