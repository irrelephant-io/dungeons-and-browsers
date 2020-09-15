using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Moq;
using Xunit;

namespace Irrelephant.DnB.Tests
{
    public class CombatFlowTests
    {
        private readonly Combat _combat;

        private readonly Mock<AiController> _defender1;

        private readonly Mock<AiController> _defender2;

        private readonly Mock<AiController> _attacker;
        
        public CombatFlowTests()
        {
            _attacker = new Mock<AiController>(new NonPlayerCharacter { Id = Guid.NewGuid(), MaxHealth = 1, Health = 1 });
            _defender1 = new Mock<AiController>(new NonPlayerCharacter { Id = Guid.NewGuid(), MaxHealth = 1, Health = 1 });
            _defender2 = new Mock<AiController>(new NonPlayerCharacter { Id = Guid.NewGuid(), MaxHealth = 1, Health = 1 });

            _combat = new Combat
            {
                Attackers = new[] { _attacker.Object },
                Defenders = new[] { _defender1.Object, _defender2.Object }
            };
        }

        [Fact]
        public void CombatEnds_WhenOneSideRemains()
        {
            Assert.False(_combat.IsOver);
            _combat.Defenders = new List<CharacterController>();
            Assert.True(_combat.IsOver);
        }

        [Fact]
        public async Task CombatShould_ResolveCharacterActions_OncePerRound()
        {
            Assert.Equal(1, _combat.Round);
            await _combat.ResolveRound();
            _attacker.Verify(a => a.Act(_combat), Times.Exactly(1));
            _defender1.Verify(a => a.Act(_combat), Times.Exactly(1));
            _defender2.Verify(a => a.Act(_combat), Times.Exactly(1));
            Assert.Equal(2, _combat.Round);
            await _combat.ResolveRound();
            _attacker.Verify(a => a.Act(_combat), Times.Exactly(2));
            _defender1.Verify(a => a.Act(_combat), Times.Exactly(2));
            _defender2.Verify(a => a.Act(_combat), Times.Exactly(2));
            Assert.Equal(3, _combat.Round);
        }

        [Fact]
        public async Task CombatShould_RemoveDeadCharacters_AtTheEndOfRound()
        {
            Assert.False(_combat.IsOver);
            _defender1.Object.Character.Health = 0;
            Assert.Equal(2, _combat.Defenders.Count());
            await _combat.ResolveRound();
            Assert.False(_combat.IsOver);
            Assert.Single(_combat.Defenders);
            _defender2.Object.Character.Health = 0;
            await _combat.ResolveRound();
            Assert.True(_combat.IsOver);
            Assert.Empty(_combat.Defenders);
        }

        [Fact]
        public async Task CombatShould_AddNewCombatants_InTheBeginningOfTheRound_WhenCombatUnderway()
        {
            await _combat.Start();
            var extraAttacker = new Mock<AiController>(new NonPlayerCharacter { Id = Guid.NewGuid() });
            var extraDefender = new Mock<AiController>(new NonPlayerCharacter { Id = Guid.NewGuid() });
            await _combat.AddAttacker(position: 0, extraAttacker.Object);
            await _combat.AddDefender(position: 0, extraDefender.Object);
            Assert.DoesNotContain(extraAttacker.Object, _combat.Combatants);
            Assert.DoesNotContain(extraDefender.Object, _combat.Combatants);
            await _combat.ResolveRound();
            Assert.Contains(extraAttacker.Object, _combat.Combatants);
            Assert.Contains(extraDefender.Object, _combat.Combatants);
        }

        [Fact]
        public async Task CombatShould_PutCombatantsInAction_UntilStarted()
        {
            var extraAttacker = new Mock<AiController>(new NonPlayerCharacter { Id = Guid.NewGuid() });
            await _combat.AddAttacker(position: 0, extraAttacker.Object);
            Assert.Contains(extraAttacker.Object, _combat.Combatants);
        }
    }
}
