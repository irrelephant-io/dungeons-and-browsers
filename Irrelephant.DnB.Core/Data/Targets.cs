using System;
using System.Linq;

namespace Irrelephant.DnB.Core.Data
{
    [Flags]
    public enum Targets
    {
        None            = 0x00,
        Self            = 0x01,
        SingleTarget    = 0x02,
        Team            = 0x04,
        MeleeRange      = 0x08,
        Friendly        = 0x10,
        Enemy           = 0x20,
        All             = 0x40
    }

    public static class TargetsHelper
    {
        public static bool Matches(this Targets viableTargets, params Targets[] targets)
        {
            var combinedTargets = targets.Aggregate(Targets.None, (acc, val) => acc | val);
            return (viableTargets & combinedTargets) != Targets.None;
        }
    }
}
