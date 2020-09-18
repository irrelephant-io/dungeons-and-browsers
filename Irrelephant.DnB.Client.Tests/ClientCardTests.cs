using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Networking;
using Irrelephant.DnB.Core.Utils;
using Moq;
using Xunit;

namespace Irrelephant.DnB.Client.Tests
{
    public class ClientCardTests
    {
        private Guid _effectId = Guid.NewGuid();

        private readonly ClientCard _card;

        private readonly Mock<ClientCombat> _combat;

        private readonly Mock<ClientPlayerCharacter> _myCharacter;

        private readonly Mock<ITargetProvider> _targetProvider;

        private static readonly Guid MockCharacterId = Guid.NewGuid();
        private readonly IEnumerable<Character> _targets = new List<Character> {
            new NonPlayerCharacter { Id = MockCharacterId }

        };

        public ClientCardTests()
        {
            _card = new ClientCard { ActionCost = 1, Id = Guid.NewGuid(), Effects = new[] { new ClientEffect(_effectId, "Test", Targets.All) } };
            _targetProvider = new Mock<ITargetProvider>();
            _targetProvider.Setup(it => it.PickTarget(It.IsAny<Effect>())).ReturnsAsync(_targets);
            _combat = new Mock<ClientCombat>();
            _myCharacter = new Mock<ClientPlayerCharacter>();
            _myCharacter.SetupGet(x => x.ClientCombat).Returns(_combat.Object);
            _myCharacter.SetupGet(x => x.Actions).Returns(100);
            _myCharacter.SetupGet(x => x.Hand).Returns(_card.ArrayOf());
        }

        [Fact]
        public async Task ShouldBeAble_ToPlayCard()
        {
            await _card.Play(_myCharacter.Object, _targetProvider.Object);
            _combat.Verify(combat => combat.PlayCard(It.IsAny<CardTargets>()), Times.Once);
        }

        [Fact]
        public async Task ShouldValidate_CardEnergyCost()
        {
            _myCharacter.SetupGet(x => x.Actions).Returns(0);
            await Assert.ThrowsAsync<NotEnoughActionsException>(() =>
                _card.Play(_myCharacter.Object, _targetProvider.Object)
            );
        }
    }
}
