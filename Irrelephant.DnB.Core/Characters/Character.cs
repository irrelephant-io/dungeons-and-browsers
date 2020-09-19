using System;
using System.Threading.Tasks;

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

        public event Action OnUpdate;

        public event Action OnDeath;

        public virtual Task DealDamage(int amount, bool ignoreArmor = false)
        {
            if (!ignoreArmor)
            {
                var armorDamage = Math.Min(Armor, amount);
                Armor -= armorDamage;
                amount -= armorDamage;
            }

            Health -= Math.Min(Health, amount);

            if (!IsAlive)
            {
                Die();
            }

            OnUpdate?.Invoke();
            return Task.CompletedTask;
        }

        protected virtual Task Die()
        {
            OnDeath?.Invoke();
            return Task.CompletedTask;
        }

        public bool IsAlive => !CanDie || Health > 0;

        public bool CanDie => MaxHealth > 0;
    }
}
