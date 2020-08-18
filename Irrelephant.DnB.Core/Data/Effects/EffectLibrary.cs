using Irrelephant.DnB.Core.Data.Effects.Library;

namespace Irrelephant.DnB.Core.Data.Effects
{
    public class EffectLibrary
    {
        public static Effect DealSmallDamage = new DealDamageEffect(8);
        public static Effect DealMediumDamage = new DealDamageEffect(14);
        public static Effect DealHeavyDamage = new DealDamageEffect(22);

        public static Effect AddBlock = new AddArmorEffect(10);
        public static Effect AddHeavyBlock = new AddArmorEffect(20);
    }
}
