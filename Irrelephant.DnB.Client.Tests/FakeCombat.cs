using System;
using Irrelephant.DnB.Core.Networking;

namespace Irrelephant.DnB.Client.Tests
{
    public static class FakeCombat
    {
        public static int Turn = 3;

        public static Guid Id = new Guid("e0d529e8-3108-49c8-a44e-aa151f5d53f6");

        public static class Attacker
        {
            public static Guid Id = new Guid("6223d4e5-875e-423c-bfdb-83292dab3b4b");
            public static string Name = "William the Mighty";
            public static string GraphicId = "player-0";
            public static int Health = 45;
            public static int MaxHealth = 60;
            public static string Intent = "Beat the shit out of everyone";
        }

        public static class Defender
        {
            public static Guid Id = new Guid("10adb33a-e0a9-49cd-ace0-deeafe526dad");
            public static string Name = "Johnathan the Pretty";
            public static string GraphicId = "player-0";
            public static int Health = 32;
            public static int MaxHealth = 55;
            public static string Intent = "Seduce all the ladies";
        }

        public static CharacterSnapshot CharacterUpdate => new CharacterSnapshot
        {
            Id = Attacker.Id,
            GraphicId = Attacker.GraphicId,
            Name = Attacker.Name,
            Health = Attacker.Health - 10,
            MaxHealth = Attacker.MaxHealth,
            Intent = Attacker.Intent
        };


        public static CombatSnapshot Object => new CombatSnapshot
        {
            Id = Id,
            Turn = Turn,
            ActiveCharacterId = Attacker.Id,
            Attackers = new[] {
                new CharacterSnapshot {
                    Id = Attacker.Id,
                    GraphicId = Attacker.GraphicId,
                    Name = Attacker.Name,
                    Health = Attacker.Health,
                    MaxHealth = Attacker.MaxHealth,
                    Intent = Attacker.Intent
                }
            },
            Defenders = new[] {
                new CharacterSnapshot {
                    Id = Defender.Id,
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
