using System.Linq;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Networking;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Server.SampleData
{
    public static class CombatFactory
    {
        public static CombatSnapshot GetSnapshot(this Combat combat)
        {
            return new CombatSnapshot
            {
                Id = combat.CombatId,
                Turn = combat.Round,
                Attackers = combat.Attackers.Select(cc => cc.Character).Select(character => new CharacterSnapshot
                {
                    Id = character.Id,
                    Name = character.Name,
                    GraphicId = character.GraphicId,
                    Health = character.Health,
                    MaxHealth = character.MaxHealth
                }).ToArray(),
                Defenders = combat.Defenders.Select(cc => cc.Character).Select(character =>
                    new CharacterSnapshot
                    {
                        Id = character.Id,
                        Name = character.Name,
                        GraphicId = character.GraphicId,
                        Health = character.Health,
                        MaxHealth = character.MaxHealth
                    }).ToArray()
            };
        }

        public static Combat BuildCombat()
        {
            var player = new PlayerCharacterController(SetupPlayer());
            var combat = new Combat
            {
                Attackers = new CharacterController[]
                {
                    player,
                    new AiController(CharacterLibrary.VileGoblin)
                },
                Defenders = new[]
                {
                    new AiController(CharacterLibrary.RagingOrc),
                    new AiController(CharacterLibrary.WretchedGoblin)
                }
            };
            combat.Start();
            return combat;
        }

        private static PlayerCharacter SetupPlayer()
        {
            var playerHand = new Card
            {
                GraphicId = "resolute-strike",
                Name = "Resolute Strike",
                ActionCost = 2,
                Effects = new[] { EffectLibrary.Card.AddBlock, EffectLibrary.Card.DealSmallMeleeDamage }
            }.Copies(2)
            .Union(
                new Card
                {
                    GraphicId = "shiv-throw",
                    Name = "Shiv Throw",
                    ActionCost = 1,
                    Effects = new[] { EffectLibrary.Card.DealSmallDamage }
                }.Copies(2)
            )
            .Union(
                new Card
                {
                    GraphicId = "reckless-assault",
                    Name = "Reckless Assault",
                    ActionCost = 1,
                    Effects = new[] {
                        EffectLibrary.Card.SelfDamageSmall, EffectLibrary.Card.DealMediumMeleeDamage
                    }
                }.Copies(1))
            .Union(
                new Card
                {
                    GraphicId = "tactical-adaptation",
                    Name = "Tactical Adaptation",
                    ActionCost = 1,
                    Effects = new[] {
                        EffectLibrary.Card.DrawTwoCards
                    }
                }.Copies(2))
            .Union(
                new Card
                {
                    GraphicId = "rampage",
                    Name = "RAMPAGE!",
                    ActionCost = 2,
                    Effects = new[] {
                        EffectLibrary.Card.SmallDamageToAllies,
                        EffectLibrary.Card.MediumDamageToEnemies
                    }
                }.Copies(2))
            .Union(
                new Card
                {
                    GraphicId = "defend",
                    Name = "Defend",
                    ActionCost = 1,
                    Effects = new[] {
                        EffectLibrary.Card.AddMediumBlock
                    }
                }.Copies(3))
            .Union(new Card
            {
                GraphicId = "strike",
                Name = "Strike",
                ActionCost = 1,
                Effects = new[] {
                    EffectLibrary.Card.DealSmallMeleeDamage
                }
            }.Copies(3));
            return new PlayerCharacter
            {
                GraphicId = "player-0",
                Name = "Player",
                MaxHealth = 70,
                Health = 70,
                EnergyMax = 4,
                DrawLimit = 6,
                DiscardPile = playerHand
            };
        }
    }
}
