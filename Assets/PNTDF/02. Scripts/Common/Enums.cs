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
        StarbornOrbCountBonus = 5,
        PhysicalAttackPowerMultiplier = 6,
        LowHpEnemyDamageMultiplier = 7,
    }

    public enum ETooltipLayout
    {
        None            = 0,
        ShopSlot        = 1,
        SynergySlot     = 2,
        PartySlot       = 3,
        PaletteSlot     = 4,
        ShopLevel       = 5,
        Hero            = 6,
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
        Enrager         = 3,
        Tanker          = 4,
        Blinker         = 5,
        Hexer           = 6,
        Summoner        = 7,
        Swarmer         = 8,
        Baby            = 9,
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

    public enum EEnemyAbility
    {
        None                = 0,
        Normal              = 1,
        Enrager             = 2,
        Tanker              = 3,
        Blinker             = 4,
        Hexer               = 5,
        Summoner            = 6,
        Swarmer             = 7,
        Baby                = 8,
    }

    public enum EEnemyTriggerType
    {
        None                = 0,
        OnSpawn             = 1,
        OnDeath             = 2,
        OnHit               = 3,
        OnHpThreshold       = 4,
        OnInterval          = 5,
        OnPassive           = 6,
    }

    public enum EHeroStat
    {
        PhysicalAttackPower             = 0,
        MagicAttackPower                = 1,
        AttackCooldown                  = 2,
        AttackRange                     = 3,
        PhysicalFlatPenetration         = 4,
        PhysicalPercentPenetration      = 5,
        MagicFlatPenetration            = 6,
        MagicPercentPenetration         = 7,
    }

    public enum EHeroStatModifierOperation
    {
        Flat                = 0,
        Additive            = 1,
        Multiply            = 2,
    }

    public enum ESound
    {
        BGM                 = 0,
        SFX                 = 1,
    }

    public enum EHeroType
    {
        Swordman            = 0,
        Barbarian           = 1,
        Slayer              = 2,
        Guardian            = 3,
        Paladin             = 4,

        Raven               = 5,
        Thief               = 6,

        Archer              = 7,
        Handgunner          = 8,
        Shotgunner          = 9,
        Artillery           = 10,
        Sniper              = 11,
        Trickshooter        = 12,

        Magician            = 13,
        Wizard              = 14,
        Explomancer         = 15,
        Telekinetic         = 16,
        Transmuter          = 17,
        Artificer           = 18,
        MageRobo            = 19,

        Martian             = 20,
        Venusian            = 21,
        Jovian              = 22,
        Saturnian           = 23,
        Uranian             = 24,

        Miner               = 25,
        Alchemist           = 26,
        Saint               = 27,
        Sancitifier         = 28,
        Crusader            = 29,
    }
}
