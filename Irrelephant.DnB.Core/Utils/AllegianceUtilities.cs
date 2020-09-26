using System;
using System.Collections.Generic;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Core.Utils
{
    public static class AllegianceUtilities
    {
        public static IEnumerable<CharacterController> GetTeamIn(this CharacterController controller, Combat combat)
        {
            if (combat.Defenders.Contains(controller))
            {
                return combat.Defenders;
            }

            if (combat.Attackers.Contains(controller))
            {
                return combat.Attackers;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static IEnumerable<CharacterController> GetOpposingTeamIn(this CharacterController controller, Combat combat)
        {
            if (combat.Defenders.Contains(controller))
            {
                return combat.Attackers;
            }

            if (combat.Attackers.Contains(controller))
            {
                return combat.Defenders;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
