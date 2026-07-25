using System;

namespace PNTD
{
    public enum ETier
    {
        None    = 0,
        Tier1   = 1,
        Tier2   = 2,
        Tier3   = 3,
    }

    [Flags]
    public enum ESynergy
    {
        None        = 0,
        
        Warrior     = 1 << 0,
        Ranger      = 1 << 1,
        Mage        = 1 << 2,
        Plunder     = 1 << 3,
        Cleric      = 1 << 4,
        Rogue       = 1 << 5,
        StarBorn    = 1 << 6,
    }

    public enum ESynergyEffect
    {
        
    }
}