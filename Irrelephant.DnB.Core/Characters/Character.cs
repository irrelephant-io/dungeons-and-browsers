using System;

namespace Irrelephant.DnB.Core.Characters
{
    public abstract class Character
    {
        public Guid Id { get; set; }

        public string GraphicId { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Armor { get; set; }

        public string Name { get; set; }

        public void DealDamage(int amount, bool ignoreArmor = false)
        {
            if (!ignoreArmor)
            {
                var armorDamage = Math.Min(Armor, amount);
                Armor -= armorDamage;
                amount -= armorDamage;
            }

            Health -= Math.Min(Health, amount);
        }

        public bool IsAlive => !CanDie || Health > 0;

        public bool CanDie => MaxHealth > 0;
    }
}
