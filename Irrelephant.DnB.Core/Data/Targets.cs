using System;

namespace Irrelephant.DnB.Core.Data
{
    [Flags]
    public enum Targets
    {
        None            = 0x00,
        Self            = 0x01,
        SingleTarget    = 0x02,
        Team            = 0x04,
        Friendly        = 0x08,
        Enemy           = 0x10,
        All             = 0x20
    }
}
