namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealDamageToEnemiesEffect : DealDamageEffect
    {
        public override string Name => $"Deal {Damage} damage to enemies.";

        public override Targets ValidTargets => Targets.Enemy | Targets.Team;

        public override EffectType EffectType => EffectType.Debuff;

        public DealDamageToEnemiesEffect(int damage) : base(damage)
        {
        }
    }
}
