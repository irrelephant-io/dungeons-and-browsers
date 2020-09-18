using System.Collections.Generic;
using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Core.Characters
{
    public class NonPlayerCharacter : Character
    {
        public virtual string Intent { get; set; }
        public virtual IEnumerable<Effect> ActionPool { get; set; }
    }
}
