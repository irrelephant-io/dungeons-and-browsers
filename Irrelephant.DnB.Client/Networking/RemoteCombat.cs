using System.Linq;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Networking;

namespace Irrelephant.DnB.Client.Networking
{
    public class RemoteCombat : Combat
    {
        private readonly IRemoteCombatListener _remoteCombatListener;

        public RemoteCombat(IRemoteCombatListener remoteCombatListener)
        {
            _remoteCombatListener = remoteCombatListener;
            _remoteCombatListener.NotifyJoined();
            _remoteCombatListener.OnJoinedCombat += OnJoinedCombat;
            _remoteCombatListener.OnCharacterUpdated += OnCharacterUpdated;
        }

        private void OnJoinedCombat(CombatSnapshot snapshot)
        {
            Attackers = snapshot.Attackers.Select(c => new AiController(MapCharacter(c))).ToArray();
            Defenders = snapshot.Defenders.Select(c => new AiController(MapCharacter(c))).ToArray();
        }

        private void OnCharacterUpdated(CharacterSnapshot snapshot)
        {
            var characterToUpdate = FindCharacterById(snapshot.Id);
            characterToUpdate.Health = snapshot.Health;
            characterToUpdate.MaxHealth = snapshot.MaxHealth;
            characterToUpdate.GraphicId = snapshot.GraphicId;
            characterToUpdate.Name = snapshot.Name;
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
