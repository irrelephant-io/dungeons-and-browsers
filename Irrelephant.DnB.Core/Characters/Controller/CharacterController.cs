﻿using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Core.Characters.Controller
{
    public abstract class CharacterController
    {
        public Character Character { get; }

        protected CharacterController(Character character)
        {
            Character = character;
        }

        public virtual Task Act(Combat combat)
        {
            Console.WriteLine($"{Character.Name} acts!");
            return Task.CompletedTask;
        }
    }
}
