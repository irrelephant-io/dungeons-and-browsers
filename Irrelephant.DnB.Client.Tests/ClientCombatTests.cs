using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Irrelephant.DnB.Client.Tests.Mocks;
using Irrelephant.DnB.Core.Exceptions;
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
            await AssertExtensions.Eventually(() => {
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
            await AssertExtensions.Eventually(() => {
                _listener.Raise(l => l.OnCharacterUpdated += null, FakeCombat.CharacterUpdate);
                Assert.Equal(FakeCombat.CharacterUpdate.Health, _combat.Attackers.Single().Character.Health);
            });
        }

        [Fact]
        public async Task ShouldGetNotified_WhenYourTurnStarts()
        {
            await StableState();
            Assert.False(_combat.MyTurn);
            await AssertExtensions.Eventually(() => {
                Assert.True(_combat.MyTurn);
            });
        }

        [Fact]
        public async Task ShouldBeAbleToAct_OnlyWhenYourTurn()
        {
            await StableState();
            await Assert.ThrowsAsync<NotMyTurnException>(() => _combat.EndTurn());
            await MyTurn();
            await _combat.EndTurn();
        }

        [Fact]
        public async Task ShouldFireUpdate_OnAnyRemoteUpdateToCombat()
        {
            var isFired = false;
            _combat.OnUpdate += () => isFired = true;
            await AssertExtensions.Eventually(() => {
                Assert.True(isFired);
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
