﻿using System.Collections.Generic;
using Irrelephant.DnB.Core.Data;

namespace Irrelephant.DnB.Core.Cards
{
    public class Card
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public int ActionCost { get; set; }

        public IEnumerable<Effect> Effects { get; set; }
    }
}