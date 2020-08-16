using System;

namespace Irrelephant.DnB.Core.Characters
{
    public abstract class Character
    {
        public string Id { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Armor { get; set; }

        public string Name { get; set; }

        public void Pass()
        {

        }

        public abstract void TakeTurn();
    }
}
