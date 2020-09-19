using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Irrelephant.DnB.Client.Tests.Mocks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.Utils;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.Tests.Utilities;
using Moq;
using Xunit;

namespace Irrelephant.DnB.Client.Tests
{
    public class ClientCombatTests
    {
        private readonly ClientCombat _combat;

        private readonly Mock<IRemoteCombatListener> _listener;

        public ClientCombatTests()
        {
            _listener = new RemoteCombatListenerMock().SetupMock();
            _combat = new ClientCombat(_listener.Object);
        }

        [Fact]
        public void ShouldDispatchJoinMessage_WhenConstructed()
        {
            _listener.Verify(l => l.NotifyJoinedAsync(), Times.Once);
        }

        [Fact]
        public async Task ShouldFetchCharacters_OnJoin()
        {
            await AssertUtilities.Eventually(() => {
                Assert.NotEmpty(_combat.Defenders);
                Assert.NotEmpty(_combat.Attackers);
                Assert.False(_combat.IsOver);
                Assert.True(_combat.IsReady);
            });
        }

        [Fact]
        public async Task ShouldUpdateCharacters_WhenReceivingUpdateEvent()
        {
            await StableState();
            await AssertUtilities.Eventually(() => {
                _listener.Raise(l => l.OnCharacterUpdated += null, FakeCombat.CharacterUpdate);
                Assert.Equal(FakeCombat.CharacterUpdate.Health, _combat.Attackers.Single().Character.Health);
            });
        }

        [Fact]
        public async Task ShouldReceiveMyCharacterId_WhenJoiningCombat()
        {
            await AssertUtilities.Eventually(() => {
                Assert.Equal(FakeCombat.Object.ActiveCharacterId, _combat.MyId);
                var pcController = _combat.Attackers.First(c => c.Character.Id == _combat.MyId);
                Assert.IsAssignableFrom<PlayerCharacterController>(pcController);
                Assert.IsAssignableFrom<PlayerCharacter>(pcController.Character);
            });
        }

        [Fact]
        public async Task ShouldGetNotified_WhenYourTurnStarts()
        {
            await StableState();
            Assert.False(_combat.MyTurn);
            await AssertUtilities.Eventually(() => {
                Assert.True(_combat.MyTurn);
            });
        }

        [Fact]
        public async Task ShouldBeAbleToAct_OnlyDuringYourTurn()
        {
            await StableState();
            await Assert.ThrowsAsync<NotMyTurnException>(() => _combat.EndTurn());
            await MyTurn();
            await _combat.EndTurn();
        }

        [Fact]
        public async Task ShouldFillOutDeckForMyCharacter_OnJoiningCombat()
        {
            await StableState();
            var myChar = _combat.MyCharacter;
            Assert.NotEmpty(myChar.DiscardPile);
            Assert.Equal(FakeCombat.Deck.Card1.CardId, myChar.DiscardPile.Single().Id);
            Assert.NotEmpty(myChar.DrawPile);
            Assert.Equal(FakeCombat.Deck.Card2.CardId, myChar.DrawPile.Single().Id);
            Assert.NotEmpty(myChar.Hand);
            Assert.Equal(FakeCombat.Deck.Card3.CardId, myChar.Hand.Single().Id);
        }

        [Fact]
        public async Task ShouldFireUpdate_OnAnyRemoteUpdateToCombat()
        {
            var isFired = false;
            _combat.OnUpdate += () => isFired = true;
            await AssertUtilities.Eventually(() => {
                Assert.True(isFired);
            });
        }

        [Fact]
        public async Task ShouldDrawCard_WhenReceivingDrawCardMessage()
        {
            await StableState();
            _listener.Raise(l => l.OnDrawCard += null, FakeCombat.Deck.Card2.CardId);
            var myChar = _combat.MyCharacter;
            await AssertUtilities.Eventually(() => {
                Assert.Contains(myChar.Hand, c => c.Id == FakeCombat.Deck.Card2.CardId);
                Assert.DoesNotContain(myChar.DrawPile, c => c.Id == FakeCombat.Deck.Card2.CardId);
            });
        }

        [Fact]
        public async Task ShouldDiscardCard_WhenReceivingDiscardCardMessage()
        {
            await StableState();
            _listener.Raise(l => l.OnDiscardCard += null, FakeCombat.Deck.Card3.CardId);
            var myChar = _combat.MyCharacter;
            await AssertUtilities.Eventually(() => {
                Assert.DoesNotContain(myChar.Hand, c => c.Id == FakeCombat.Deck.Card3.CardId);
                Assert.Contains(myChar.DiscardPile, c => c.Id == FakeCombat.Deck.Card3.CardId);
            });
        }

        [Fact]
        public async Task ShouldPutAllCardIntoDrawPile_OnReshuffle()
        {
            await StableState();
            _listener.Raise(l => l.OnReshuffleDiscardPile += null);
            var myChar = _combat.MyCharacter;
            await AssertUtilities.Eventually(() => {
                Assert.Empty(myChar.DiscardPile);
                Assert.Contains(myChar.DrawPile, c => c.Id == FakeCombat.Deck.Card1.CardId);
                Assert.Contains(myChar.DrawPile, c => c.Id == FakeCombat.Deck.Card2.CardId);
            });
        }

        [Fact]
        public async Task ShouldGetResponse_AfterPlayingCard()
        {
            await MyTurn();
            var card = _combat.MyCharacter.Hand.First();
            var targets = new CardTargets {
                CardId = card.Id,
                EffectTargets = card.Effects.ToDictionary(e => e.Id, _ => _combat.Defenders.First().Character.Id.ArrayOf().ToArray())
            };
            await _combat.PlayCard(targets);
            await AssertUtilities.Eventually(() => {
                Assert.Empty(_combat.MyCharacter.Hand);
                Assert.Equal(_combat.MyCharacter.ActionsMax - card.ActionCost, _combat.MyCharacter.Actions);
            });
        }
        
        private Task StableState()
        {
            return AwaitState(() => _combat.IsReady);
        }

        private Task MyTurn()
        {
            return AwaitState(() => _combat.MyTurn);
        }

        private async Task AwaitState(Func<bool> condition)
        {
            while (true)
            {
                await Task.Delay(RemoteCombatListenerMock.NetworkDelay);
                if (condition())
                {
                    return;
                }
            }
        }
    }
}
