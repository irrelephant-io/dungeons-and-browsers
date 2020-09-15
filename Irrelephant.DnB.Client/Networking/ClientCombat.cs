using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Networking;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientCombat : Combat
    {
        public bool MyTurn { get; private set; }

        public bool IsReady { get; private set; }

        public Guid MyId { get; private set; }

        public PlayerCharacter MyCharacter => (PlayerCharacter)FindCharacterById(MyId);

        private readonly IRemoteCombatListener _remoteCombatListener;

        public ClientCombat(IRemoteCombatListener remoteCombatListener)
        {
            _remoteCombatListener = remoteCombatListener;
            _remoteCombatListener.NotifyJoinedAsync();
            _remoteCombatListener.OnJoinedCombat += OnJoinedCombat;
            _remoteCombatListener.OnCharacterUpdated += OnCharacterUpdated;
            _remoteCombatListener.OnMyTurn += OnMyTurn;
            _remoteCombatListener.OnDrawCard += OnDrawCard;
            _remoteCombatListener.OnDiscardCard += OnDiscardCard;
            _remoteCombatListener.OnReshuffleDiscardPile += OnReshuffleDiscardPile;
        }

        private void OnReshuffleDiscardPile()
        {
            var myChar = MyCharacter;
            myChar.DrawPile = myChar.DrawPile.Union(myChar.DiscardPile).ToArray();
            myChar.DiscardPile = Enumerable.Empty<Card>().ToArray();
        }

        private void OnDiscardCard(Guid cardId)
        {
            var myChar = MyCharacter;
            var discardedCard = myChar.Hand.First(card => card.Id == cardId);
            myChar.Hand = myChar.Hand.Where(c => c != discardedCard).ToArray();
            myChar.DiscardPile = myChar.DiscardPile.Union(discardedCard.ArrayOf()).ToArray();
        }

        private void OnDrawCard(Guid cardId)
        {
            var myChar = MyCharacter;
            var drawnCard = myChar.DrawPile.First(card => card.Id == cardId);
            myChar.DrawPile = myChar.DrawPile.Where(c => c != drawnCard).ToArray();
            myChar.Hand = myChar.Hand.Union(drawnCard.ArrayOf()).ToArray();
        }

        public async Task EndTurn()
        {
            await _remoteCombatListener.NotifyEndTurnAsync(CombatId);
            NotifyUpdate();
        }

        private void OnMyTurn()
        {
            MyTurn = true;
            NotifyUpdate();
        }

        private void OnJoinedCombat(CombatSnapshot snapshot)
        {
            CombatId = snapshot.Id;
            MyId = snapshot.ActiveCharacterId;
            Attackers = snapshot.Attackers.Select(MapCharacter).ToArray();
            Defenders = snapshot.Defenders.Select(MapCharacter).ToArray();
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

        private CharacterController MapCharacter(CharacterSnapshot snapshot)
        {
            var character = snapshot.Id == MyId
                ? new PlayerCharacter() as Character
                : new NonPlayerCharacter();

            character.Id = snapshot.Id;
            character.Name = snapshot.Name;
            character.Health = snapshot.Health;
            character.MaxHealth = snapshot.MaxHealth;
            character.GraphicId = snapshot.GraphicId;

            if (character is PlayerCharacter pc)
            {
                pc.Hand = snapshot.Deck.Hand.Select(MapCard).ToArray();
                pc.DiscardPile = snapshot.Deck.DiscardPile.Select(MapCard).ToArray();
                pc.DrawPile = snapshot.Deck.DrawPile.Select(MapCard).ToArray();
                return new PlayerCharacterController(pc);
            }

            return new RemoteCharacterController(character);
        }

        private Card MapCard(CardSnapshot snapshot)
        {
            return new ClientCard
            {
                Id = snapshot.Id,
                ActionCost = snapshot.ActionCost,
                GraphicId = snapshot.GraphicId,
                Name = snapshot.Name,
                RemoteText = snapshot.Text
            };
        }
    }
}
