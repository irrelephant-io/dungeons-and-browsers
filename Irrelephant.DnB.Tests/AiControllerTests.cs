using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Tests.Utilities;
using Moq;
using Xunit;

namespace Irrelephant.DnB.Tests
{
    public class AiControllerTests
    {
        private readonly AiController _controller;

        private readonly Mock<Effect> _effect1;

        private readonly Mock<Effect> _effect2;

        private readonly Mock<Combat> _combat;

        private readonly Mock<AiController> _mockEnemy;

        public AiControllerTests()
        {
            _effect1 = new Mock<Effect>();
            _effect1.Setup(e => e.ValidTargets).Returns(Targets.Self);
            _effect2 = new Mock<Effect>();
            _effect2.Setup(e => e.ValidTargets).Returns(Targets.SingleTarget | Targets.Enemy);
            var character = new NonPlayerCharacter
            {
                ActionPool = new[]
                {
                    _effect1.Object,
                    _effect2.Object
                }
            };
            _controller = new AiController(character);
            _combat = new Mock<Combat>();
            _mockEnemy = new Mock<AiController>(new NonPlayerCharacter());
            _combat.SetupGet(combat => combat.Attackers).Returns(new List<CharacterController> { _mockEnemy.Object });
            _combat.SetupGet(combat => combat.Defenders).Returns(new List<CharacterController> { _controller });
        }

        [Fact]
        public async Task AiController_ShouldQueue_ActionsFromPool()
        {
            await _controller.Act(_combat.Object);
            _effect1.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Once);
            _effect2.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Never);
            await _controller.Act(_combat.Object);
            _effect1.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Once);
            _effect2.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Once);
            await _controller.Act(_combat.Object);
            _effect1.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Exactly(2));
            _effect2.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Once);
            await _controller.Act(_combat.Object);
            _effect1.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Exactly(2));
            _effect2.Verify(effect => effect.Apply(It.IsAny<IEnumerable<Character>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AiController_ShouldResolveTargetedEffects()
        {
            var targetList = new List<Character>();
            void Callback(IEnumerable<Character> targets) => targetList.AddRange(targets);

            var selfEffect = BuildMockEffect(Targets.Self, Callback);
            var friendliesEffect = BuildMockEffect(Targets.Friendly | Targets.Team, Callback);
            var enemiesEffect = BuildMockEffect(Targets.Enemy | Targets.Team, Callback);
            var friendlyEffect = BuildMockEffect(Targets.Friendly | Targets.SingleTarget, Callback);
            var enemyEffect = BuildMockEffect(Targets.Enemy | Targets.SingleTarget, Callback);
            var allEffect = BuildMockEffect(Targets.All, Callback);

            var controller = new AiController(new NonPlayerCharacter
            {
                ActionPool = new []
                {
                    selfEffect.Object,
                    friendliesEffect.Object,
                    enemiesEffect.Object,
                    friendlyEffect.Object,
                    enemyEffect.Object,
                    allEffect.Object
                }
            });

            var mockFriendly = new Mock<AiController>(new NonPlayerCharacter());
            var mockEnemy2 = new Mock<AiController>(new NonPlayerCharacter());

            var combat = new Mock<Combat>();
            combat.SetupGet(c => c.Attackers).Returns(new List<CharacterController> { _mockEnemy.Object, mockEnemy2.Object });
            combat.SetupGet(c => c.Defenders).Returns(new List<CharacterController> { controller, mockFriendly.Object });

            await controller.Act(combat.Object);
            AssertionUtilities.OnlyContains(targetList, new [] { controller.Character });
            targetList.Clear();

            await controller.Act(combat.Object);
            AssertionUtilities.OnlyContains(targetList, new[] { controller.Character, mockFriendly.Object.Character });
            targetList.Clear();

            await controller.Act(combat.Object);
            AssertionUtilities.OnlyContains(targetList, new[] { _mockEnemy.Object.Character, mockEnemy2.Object.Character });
            targetList.Clear();

            await controller.Act(combat.Object);
            AssertionUtilities.OnlyContains(targetList, new[] { controller.Character });
            targetList.Clear();

            await controller.Act(combat.Object);
            AssertionUtilities.OnlyContains(targetList, new[] { _mockEnemy.Object.Character });
            targetList.Clear();


            await controller.Act(combat.Object);
            AssertionUtilities.OnlyContains(targetList, new[] {
                controller.Character,
                mockFriendly.Object.Character,
                _mockEnemy.Object.Character,
                mockEnemy2.Object.Character
            });
        }

        private Mock<Effect> BuildMockEffect(Targets targets, Action<IEnumerable<Character>> callback)
        {
            var mock = new Mock<Effect>();
            mock.SetupGet(e => e.ValidTargets).Returns(targets);
            mock.Setup(e => e.Apply(It.IsAny<IEnumerable<Character>>())).Callback(callback);
            return mock;
        }
    }
}
