using System;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.Client.Tests
{
    public static class FakeCombat
    {
        public static int Turn = 3;

        public static Guid Id = new Guid("e0d529e8-3108-49c8-a44e-aa151f5d53f6");

        public static class Deck
        {
            public static class Card1
            {
                public static Guid CardId = new Guid("75bf3700-e59b-4648-89b6-a230644ba881");
                public static string Name = "Strike";
                public static int ActionCost = 1;
                public static string GraphicId = "strike";
                public static string Text = "Deal some melee damage";
            }
            public static class Card2
            {
                public static Guid CardId = new Guid("cdf41fcb-3df9-42e3-b11d-867112f31486");
                public static string Name = "Defend";
                public static int ActionCost = 1;
                public static string GraphicId = "defend";
                public static string Text = "Put your shield up";
            }
            public static class Card3
            {
                public static Guid CardId = new Guid("e7753dd7-a902-4fc0-b624-f39c2e125005");
                public static string Name = "Hat Trick";
                public static int ActionCost = 2;
                public static string GraphicId = "hat-trick";
                public static string Text = "Make an awesome move";
            }
        }

        public static class Attacker
        {
            public static Guid CharacterId = new Guid("6223d4e5-875e-423c-bfdb-83292dab3b4b");
            public static string Name = "William the Mighty";
            public static string GraphicId = "player-0";
            public static int Health = 45;
            public static int MaxHealth = 60;
            public static string Intent = "Beat the shit out of everyone";
        }

        public static class Defender
        {
            public static Guid CharacterId = new Guid("10adb33a-e0a9-49cd-ace0-deeafe526dad");
            public static string Name = "Johnathan the Pretty";
            public static string GraphicId = "player-0";
            public static int Health = 32;
            public static int MaxHealth = 55;
            public static string Intent = "Seduce all the ladies";
        }

        public static CharacterSnapshot CharacterUpdate => new CharacterSnapshot
        {
            Id = Attacker.CharacterId,
            GraphicId = Attacker.GraphicId,
            Name = Attacker.Name,
            Health = Attacker.Health - 10,
            MaxHealth = Attacker.MaxHealth,
            Intent = Attacker.Intent,
            Actions = 4,
            ActionsMax = 4
        };


        public static CombatSnapshot Object => new CombatSnapshot
        {
            Id = Id,
            Turn = Turn,
            ActiveCharacterId = Attacker.CharacterId,
            PendingDefenders = new CharacterSnapshot[0],
            PendingAttackers = new CharacterSnapshot[0],
            Attackers = new[] {
                new CharacterSnapshot {
                    Id = Attacker.CharacterId,
                    GraphicId = Attacker.GraphicId,
                    Name = Attacker.Name,
                    Health = Attacker.Health,
                    MaxHealth = Attacker.MaxHealth,
                    Intent = Attacker.Intent,
                    Actions = 4,
                    ActionsMax = 4,
                    Deck = new DeckSnapshot {
                        DiscardPile = new[] {
                            new CardSnapshot {
                                Id = Deck.Card1.CardId,
                                ActionCost = Deck.Card1.ActionCost,
                                Name = Deck.Card1.Name,
                                GraphicId = Deck.Card1.GraphicId,
                                Text = Deck.Card1.Text,
                                Effects = new EffectSnapshot[0]
                            }
                        },
                        DrawPile = new[] {
                            new CardSnapshot {
                                Id = Deck.Card2.CardId,
                                ActionCost = Deck.Card2.ActionCost,
                                Name = Deck.Card2.Name,
                                GraphicId = Deck.Card2.GraphicId,
                                Text = Deck.Card2.Text,
                                Effects = new EffectSnapshot[0]
                            }
                        },
                        Hand = new[] {
                            new CardSnapshot {
                                Id = Deck.Card3.CardId,
                                ActionCost = Deck.Card3.ActionCost,
                                Name = Deck.Card3.Name,
                                GraphicId = Deck.Card3.GraphicId,
                                Text = Deck.Card3.Text,
                                Effects = new EffectSnapshot[0]
                            }
                        }
                    }
                }
            },
            Defenders = new[] {
                new CharacterSnapshot {
                    Id = Defender.CharacterId,
                    Name = Defender.Name,
                    GraphicId = Defender.GraphicId,
                    Health = Defender.Health,
                    MaxHealth = Defender.MaxHealth,
                    Intent = Defender.Intent
                }
            }
        };
    }
}
