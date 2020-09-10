using Irrelephant.DnB.Core.Data.Effects.Library;

namespace Irrelephant.DnB.Core.Data.Effects
{
    public class EffectLibrary
    {
        public static class Card
        {
            public static Effect DealSmallMeleeDamage = new DealMeleeDamageEffect(12);
            public static Effect DealMediumMeleeDamage = new DealMeleeDamageEffect(15);
            public static Effect DealSmallDamage = new DealDamageEffect(8);
            public static Effect AddBlock = new AddArmorEffect(10);
            public static Effect AddMediumBlock = new AddArmorEffect(14);
            public static Effect SelfDamageSmall = new DealDamageToSelfEffect(5);
            public static Effect DrawTwoCards = new DrawCardsEffect(2);
            public static Effect SmallDamageToAllies = new DealDamageToAlliesEffect(6);
            public static Effect MediumDamageToEnemies = new DealDamageToEnemiesEffect(12);
        }

        public static class Npc
        {
            public static Effect DealSmallDamage = new DealDamageEffect(8);
            public static Effect DealMediumDamage = new DealDamageEffect(14);
            public static Effect DealHeavyDamage = new DealDamageEffect(22);
            public static Effect AddBlock = new AddArmorEffect(10);
        }
    }
}
