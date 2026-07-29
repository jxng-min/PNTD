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
        AttackCooldownMultiplier = 0,
        EnemySlowOnHit = 1,
        EnemyGoldDropOnKill = 2,
        OraRangeMultiplier = 3,
        OraEffectMultiplier = 4,
    }

    public enum ETooltipLayout
    {
        None            = 0,
        ShopSlot        = 1,
        SynergySlot     = 2,
        PartySlot       = 3,
        PaletteSlot     = 4,
    }

    public enum EAttack
    {
        Physical        = 0,
        Magic           = 1,
        True            = 2,
    }

    public enum EEnemyType
    {
        Normal          = 0,
        Mini            = 1,
        Elite           = 2,
    }
}