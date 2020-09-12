using System;
using System.Collections.Generic;

namespace Irrelephant.DnB.Core.Networking
{
    public class CombatSnapshot
    {
        public Guid Id;

        public IEnumerable<CharacterSnapshot> Attackers;

        public IEnumerable<CharacterSnapshot> Defenders;

        public Guid ActiveCharacterId;

        public int Turn;
    }
}
