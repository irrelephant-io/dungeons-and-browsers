using System.Linq;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Irrelephant.DnB.Client.Networking;
using Moq;
using Xunit;

namespace Irrelephant.DnB.Client.Tests
{
    public class RemoteCombatTests
    {
        private readonly RemoteCombat _combat;

        private readonly Mock<IRemoteCombatListener> _listener;

        public const int NetworkDelay = 300;

        public RemoteCombatTests()
        {
            _listener = SetupListenerMock();
            _combat = new RemoteCombat(_listener.Object);
        }

        private Mock<IRemoteCombatListener> SetupListenerMock()
        {
            var listener = new Mock<IRemoteCombatListener>();
            listener
                .Setup(l => l.NotifyJoined())
                .Callback(async () =>
                {
                    await Task.Delay(NetworkDelay);
                    listener.Raise(l => l.OnJoinedCombat += null, FakeCombat.Object);
                })
                .Returns(Task.Delay(NetworkDelay / 2));
            return listener;
        }

        [Fact]
        public void ShouldDispatchJoinMessage_WhenConstructed()
        {
            _listener.Verify(l => l.NotifyJoined(), Times.Once);
        }

        [Fact]
        public async Task ShouldFetchCharacters_OnJoin()
        {
            await AssertExtensions.Eventually(() => {
                Assert.NotEmpty(_combat.Defenders);
                Assert.NotEmpty(_combat.Attackers);
                Assert.False(_combat.IsOver);
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

        private static Task StableState()
        {
            return Task.Delay((int)(NetworkDelay * 1.1));
        }
    }
}
