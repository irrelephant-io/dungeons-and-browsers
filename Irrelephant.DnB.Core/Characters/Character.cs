using System;

namespace Irrelephant.DnB.Core.Characters
{
    public abstract class Character
    {
        public int Health { get; set; }

        public string Name { get; set; }

        public void Pass()
        {

        }

        public abstract void TakeTurn();
    }
}
