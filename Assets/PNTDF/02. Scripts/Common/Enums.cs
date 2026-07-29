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

    public enum EStackPolicy
    {
        Stack               = 0,    // 중첩
        RefreshDuration     = 1,    // 기존 효과의 지속 시간 갱신
        ExtendDuration      = 2,    // 기존 효과의 지속 시간 중첩
        Replace             = 3,    // 대체
        KeepStrongest       = 4,    // 더 강한 효과만 유지
    }

    public enum EEffectCategory
    {
        Buff                = 0,
        Debuff              = 1,
        Control             = 2,
    }
}