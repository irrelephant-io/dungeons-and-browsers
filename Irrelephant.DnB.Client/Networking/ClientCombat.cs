using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Utils;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientCombat : Combat
    {
        public bool MyTurn { get; private set; }

        public bool IsReady { get; private set; }

        public Guid MyId { get; private set; }

        public ClientPlayerCharacter MyCharacter => (ClientPlayerCharacter)FindCharacterById(MyId);

        public ClientCombat() { }

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
            _remoteCombatListener.OnCardPlayed += OnCardPlayed;
            _remoteCombatListener.LeftCombat += OnLeftCombat;
            _remoteCombatListener.OnCharacterJoined += OnCharacterJoined;
            _remoteCombatListener.OnPendingCombat += OnPendingCombat;
        }

        private void OnPendingCombat(JoinFightMessage message)
        {
            var character = MapCharacter(message.Character);
            if (message.Side == JoinedSide.Attackers)
            {
                var attackersList = PendingAttackers.ToList();
                attackersList.Add((message.Position, character));
                PendingAttackers = attackersList;
            }
            else
            {
                var defendersList = PendingDefenders.ToList();
                defendersList.Add((message.Position, character));
                PendingDefenders = defendersList;
            }
            NotifyUpdate();
        }

        private void OnCharacterJoined(JoinFightMessage message)
        {
            var character = MapCharacter(message.Character);
            if (message.Side == JoinedSide.Attackers)
            {
                InsertNewAttacker(message.Position, character);
            }
            else
            {
                InsertNewDefender(message.Position, character);
            }
            NotifyUpdate();
        }

        private void InsertNewAttacker(int position, CharacterController character)
        {
            var newCharList = Attackers.ToList();
            var pendingList = PendingAttackers.ToList();
            var pendingChar = PendingAttackers.FirstOrDefault(pending => pending.cc.Character.Id == character.Character.Id);
            pendingList.Remove(pendingChar);
            newCharList.Insert(position, pendingChar.cc);
            Attackers = newCharList.ToArray();
        }

        private void InsertNewDefender(int position, CharacterController character)
        {
            var newCharList = Defenders.ToList();
            var pendingList = PendingDefenders.ToList();
            var pendingChar = PendingDefenders.FirstOrDefault(pending => pending.cc.Character.Id == character.Character.Id);
            pendingList.Remove(pendingChar);
            newCharList.Insert(position, pendingChar.cc);
            Defenders = newCharList.ToArray();
        }

        private void OnLeftCombat(Guid characterId)
        {
            Attackers = Attackers.Where(cc => cc.Character.Id != characterId).ToArray();
            Defenders = Defenders.Where(cc => cc.Character.Id != characterId).ToArray();
        }

        private void OnCardPlayed(Guid cardId)
        {
        }

        private void OnReshuffleDiscardPile()
        {
            var myChar = MyCharacter;
            myChar.DrawPile = myChar.DrawPile.Union(myChar.DiscardPile).ToArray();
            myChar.DiscardPile = Enumerable.Empty<Card>().ToArray();
            NotifyUpdate();
        }

        private void OnDiscardCard(Guid cardId)
        {
            var myChar = MyCharacter;
            var discardedCard = myChar.Hand.First(card => card.Id == cardId);
            myChar.Hand = myChar.Hand.Where(c => c != discardedCard).ToArray();
            myChar.DiscardPile = myChar.DiscardPile.Union(discardedCard.ArrayOf()).ToArray();
            NotifyUpdate();
        }

        private void OnDrawCard(Guid cardId)
        {
            var myChar = MyCharacter;
            var drawnCard = myChar.DrawPile.First(card => card.Id == cardId);
            myChar.DrawPile = myChar.DrawPile.Where(c => c != drawnCard).ToArray();
            myChar.Hand = myChar.Hand.Union(drawnCard.ArrayOf()).ToArray();
            NotifyUpdate();
        }

        public async Task EndTurn()
        {
            await _remoteCombatListener.NotifyEndTurnAsync();
            NotifyUpdate();
        }

        public async virtual Task PlayCard(CardTargets targets)
        {
            await _remoteCombatListener.PlayCard(targets);
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
            PendingAttackers = snapshot.PendingAttackers.Select((item, index) => (index, MapCharacter(item))).ToArray();
            Defenders = snapshot.Defenders.Select(MapCharacter).ToArray();
            PendingDefenders = snapshot.PendingDefenders.Select((item, index) => (index, MapCharacter(item))).ToArray();
            MyCharacter.ClientCombat = this;
            Console.WriteLine(MyCharacter.ClientCombat == null);
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
            characterToUpdate.Armor = snapshot.Armor;

            if (characterToUpdate is ClientPlayerCharacter pc)
            {
                pc.Actions = snapshot.Actions;
                pc.ActionsMax = snapshot.ActionsMax;
            }
            else if (characterToUpdate is NonPlayerCharacter npc)
            {
                npc.Intent = snapshot.Intent;
            }

            NotifyUpdate();
        }

        private CharacterController MapCharacter(CharacterSnapshot snapshot)
        {
            var character = snapshot.Id == MyId
                ? new ClientPlayerCharacter() as Character
                : new NonPlayerCharacter();

            character.Id = snapshot.Id;
            character.Name = snapshot.Name;
            character.Health = snapshot.Health;
            character.MaxHealth = snapshot.MaxHealth;
            character.GraphicId = snapshot.GraphicId;
            character.Armor = snapshot.Armor;

            if (character is ClientPlayerCharacter pc)
            {
                pc.Hand = snapshot.Deck.Hand.Select(MapCard).ToArray();
                pc.DiscardPile = snapshot.Deck.DiscardPile.Select(MapCard).ToArray();
                pc.DrawPile = snapshot.Deck.DrawPile.Select(MapCard).ToArray();
                pc.Actions = snapshot.Actions;
                pc.ActionsMax = snapshot.ActionsMax;
                return new PlayerCharacterController(pc);
            }
            else if (character is NonPlayerCharacter npc)
            {
                npc.Intent = snapshot.Intent;
            }

            return new ClientCharacterController(character);
        }

        private Card MapCard(CardSnapshot snapshot)
        {
            return new ClientCard
            {
                Id = snapshot.Id,
                ActionCost = snapshot.ActionCost,
                GraphicId = snapshot.GraphicId,
                Name = snapshot.Name,
                RemoteText = snapshot.Text,
                Effects = snapshot.Effects.Select(snap => new ClientEffect(snap.Id, snap.Name, snap.Targets)).ToArray()
            };
        }

    }
}
