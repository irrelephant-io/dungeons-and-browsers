using System;
using System.Collections.Generic;
using System.Linq;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Utils;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.Networking;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

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
                Attackers = combat.Attackers.Select(cc => cc.Character).Select(c => GetCharacterSnapshot(c)).ToArray(),
                Defenders = combat.Defenders.Select(cc => cc.Character).Select(c => GetCharacterSnapshot(c)).ToArray(),
                PendingAttackers = combat.PendingAttackers.Select(pair => pair.cc.Character).Select(c => GetCharacterSnapshot(c)).ToArray(),
                PendingDefenders = combat.PendingDefenders.Select(pair => pair.cc.Character).Select(c => GetCharacterSnapshot(c)).ToArray()
            };
        }

        public static CharacterSnapshot GetCharacterSnapshot(this Character character, bool sendDeck = true)
        {
            var snap = new CharacterSnapshot
            {
                Id = character.Id,
                Name = character.Name,
                GraphicId = character.GraphicId,
                Health = character.Health,
                MaxHealth = character.MaxHealth,
                Armor = character.Armor,
            };
            if (character is PlayerCharacter pc)
            {
                if (sendDeck)
                {
                    snap.Deck = GetDeckSnapshot(pc);
                }
                snap.Actions = pc.Actions;
                snap.ActionsMax = pc.ActionsMax;
            }
            else if (character is NonPlayerCharacter npc)
            {
                snap.Intent = npc.Intent;
            }

            return snap;
        }

        private static DeckSnapshot GetDeckSnapshot(PlayerCharacter character)
        {
            return new DeckSnapshot
            {
                Hand = character.Hand.Select(MapCard).ToArray(),
                DiscardPile = character.DiscardPile.Select(MapCard).ToArray(),
                DrawPile = character.DrawPile.Select(MapCard).ToArray()
            };
        }

        private static CardSnapshot MapCard(Card card)
        {
            return new CardSnapshot
            {
                Id = card.Id,
                ActionCost = card.ActionCost,
                GraphicId = card.GraphicId,
                Name = card.Name,
                Text = card.Text,
                Effects = card.Effects.Select(e => new EffectSnapshot
                {
                    Id = e.Id,
                    Name = e.Name,
                    Targets = e.ValidTargets
                }).ToArray()
            };
        }

        public static Combat BuildCombat(IServiceProvider services)
        {
            var hubContext = services.GetRequiredService<IHubContext<CombatHub>>();
            return new Combat
            {
                Attackers = new List<CharacterController>
                {
                    new RemoteAiController(hubContext, CharacterLibrary.VileGoblin.Copy())
                },
                Defenders = new List<CharacterController>
                {
                    new RemoteAiController(hubContext, CharacterLibrary.RagingOrc.Copy()),
                    new RemoteAiController(hubContext, CharacterLibrary.WretchedGoblin.Copy())
                }
            };
        }

        public static RemotePlayerCharacter SetupPlayer(IServiceProvider services)
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
            }.Copies(3))
                .ForEach(card => card.Id = Guid.NewGuid())
                .Select(card => new RemoteCard(services.GetRequiredService<IHubContext<CombatHub>>(), card))
                .ToArray();
            return new RemotePlayerCharacter(services.GetRequiredService<IHubContext<CombatHub>>())
            {
                Id = Guid.NewGuid(),
                GraphicId = "player-0",
                Name = "Player",
                MaxHealth = 70,
                Health = 70,
                ActionsMax = 4,
                DrawLimit = 6,
                DrawPile = playerHand.Shuffle()
            };
        }
    }
}
