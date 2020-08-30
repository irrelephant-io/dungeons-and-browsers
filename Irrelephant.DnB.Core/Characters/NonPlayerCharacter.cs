using System.Collections.Generic;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Core.Characters
{
    public class NonPlayerCharacter : Character
    {
        public IntentType Intent;

        public IEnumerable<Effect> ActionPool;

    }
}
