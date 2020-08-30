namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealMeleeDamageEffect : DealDamageEffect
    {
        public DealMeleeDamageEffect(int damage) : base(damage)
        {
        }

        public override string Name => $"Deal {Damage} melee damage.";

        public override Targets ValidTargets => Targets.MeleeRange;

    }
}
