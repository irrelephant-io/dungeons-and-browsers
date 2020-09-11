using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.Data.Effects.Library;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.Utils;
using Moq;
using Xunit;

namespace Irrelephant.DnB.Tests
{
    public class CardTests
    {
        private readonly Card _card;

        private readonly Mock<ITargetProvider> _targetProviderMock;

        private readonly Mock<PlayerCharacter> _character;

        private readonly Effect _effect1;

        private readonly Effect _effect2;

        public CardTests()
        {
            _effect1 = new DealMeleeDamageEffect(5);
            _effect2 = new AddArmorEffect(5);
            _character = new Mock<PlayerCharacter>();
            var effectTarget = new Mock<Character>();
            _targetProviderMock = new Mock<ITargetProvider>();
            _targetProviderMock.Setup(x => x.PickTarget(It.IsAny<Effect>()))
                .Returns(Task.FromResult<IEnumerable<Character>>(new[] { effectTarget.Object }));
            _card = new Card
            {
                Name = "Resolute strike",
                ActionCost = 1,
                Effects = new[] { _effect1, _effect2 }
            };
            _character.SetupGet(c => c.Hand).Returns(_card.ArrayOf());
            _character.SetupGet(c => c.DiscardPile).Returns(Enumerable.Empty<Card>());
        }

        [Fact]
        public async Task Card_CantBePlayed_WhenNotEnoughActions()
        {
            _character.SetupGet(x => x.Energy).Returns(0);
            await Assert.ThrowsAsync<NotEnoughActionsException>(() => _card.Play(_character.Object, _targetProviderMock.Object));
        }

        [Fact]
        public async Task Card_RequiresTargetsPerEffect_WhenPlayed()
        {
            _character.SetupGet(x => x.Energy).Returns(100);
            await _card.Play(_character.Object, _targetProviderMock.Object);
            _targetProviderMock.Verify(x => x.PickTarget(_effect1), Times.Once);
            _targetProviderMock.Verify(x => x.PickTarget(_effect2), Times.Once);
        }

        [Fact]
        public async Task Card_UsesEnergy_WhenPlayed()
        {
            _character.SetupProperty(x => x.Energy, 100);
            await _card.Play(_character.Object, _targetProviderMock.Object);
            Assert.Equal(99, _character.Object.Energy);
        }

        [Fact]
        public async Task Card_ShouldMoveToDiscard_WhenPlayed()
        {
            _character.SetupGet(x => x.Energy).Returns(100);
            await _card.Play(_character.Object, _targetProviderMock.Object);
            _character.Verify(c => c.Discard(_card), Times.Once);
        }
    }
}
