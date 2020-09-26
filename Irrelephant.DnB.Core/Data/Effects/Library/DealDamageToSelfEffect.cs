namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealDamageToSelfEffect : DealDamageEffect
    {
        public override string Name => $"Deal {Damage} damage to self.";

        public override Targets ValidTargets => Targets.Self;

        public override EffectType EffectType => EffectType.Debuff;

        public DealDamageToSelfEffect(int damage) : base(damage)
        {
        }
    }
}
