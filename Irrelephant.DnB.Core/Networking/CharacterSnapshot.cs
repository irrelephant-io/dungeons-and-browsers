using System;

namespace Irrelephant.DnB.Core.Networking
{
    public class CharacterSnapshot
    {
        public Guid Id { get; set; }

        public string GraphicId { get; set; }

        public string Name { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public string Intent { get; set; }
    }
}
