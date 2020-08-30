using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Core.Characters
{
    public static class CharacterLibrary
    {
        public static Character VileGoblin = new NonPlayerCharacter
        {
            GraphicId = "goblin-0",
            Name = "Vile Goblin", 
            Health = 30, MaxHealth = 30,
            ActionPool = new []
            {
                EffectLibrary.Npc.DealSmallDamage,
                EffectLibrary.Npc.DealSmallDamage,
                EffectLibrary.Npc.AddBlock
            }
        };

        public static Character WretchedGoblin = new NonPlayerCharacter
        {
            GraphicId = "goblin-1",
            Name = "Wretched Goblin",
            Health = 25, MaxHealth = 25,
            ActionPool = new[]
            {
                EffectLibrary.Npc.DealMediumDamage,
                EffectLibrary.Npc.AddBlock
            }
        };

        public static Character RagingOrc = new NonPlayerCharacter
        {
            GraphicId = "orc-0",
            Name = "Raging Orc",
            Health = 55,
            MaxHealth = 55,
            ActionPool = new[]
            {
                EffectLibrary.Npc.DealMediumDamage,
                EffectLibrary.Npc.DealMediumDamage,
                EffectLibrary.Npc.DealHeavyDamage
            }
        };
    }
}
