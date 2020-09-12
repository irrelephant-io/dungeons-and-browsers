using System;
using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Core.Characters
{
    public static class CharacterLibrary
    {
        public static Character VileGoblin = new NonPlayerCharacter
        {
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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
