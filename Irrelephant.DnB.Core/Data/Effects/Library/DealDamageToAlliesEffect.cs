namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealDamageToAlliesEffect : DealDamageEffect
    {
        public override string Name => $"Deal {Damage} damage to allies.";

        public override Targets ValidTargets => Targets.Friendly | Targets.Team;

        public override EffectType EffectType => EffectType.Debuff;

        public DealDamageToAlliesEffect(int damage) : base(damage)
        {
        }
    }
}
