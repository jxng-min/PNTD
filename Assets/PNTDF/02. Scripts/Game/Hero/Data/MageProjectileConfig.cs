using UnityEngine;

namespace PNTD
{
    public readonly struct MageProjectileConfig
    {
        public readonly Hero Owner;
        public readonly float DamageMultiplier;
        public readonly float Speed;
        public readonly float MaxDistance;
        public readonly float HitRadius;
        public readonly string DotEffectId;
        public readonly float DotDamageMultiplier;
        public readonly float DotTickInterval;
        public readonly float DotDuration;
        public readonly string DisableEffectId;
        public readonly float DisableChance;
        public readonly float DisableDuration;
        public readonly Color? DisableOverrideColor;
        public readonly string StunEffectId;
        public readonly float StunDuration;
        public readonly Color? StunOverrideColor;
        public readonly bool TriggerOnHitEffect;

        public MageProjectileConfig(Hero owner,
                                    float damageMultiplier,
                                    float speed,
                                    float maxDistance,
                                    float hitRadius,
                                    string dotEffectId,
                                    float dotDamageMultiplier,
                                    float dotTickInterval,
                                    float dotDuration,
                                    string disableEffectId = null,
                                    float disableChance = 0f,
                                    float disableDuration = 0f,
                                    Color? disableOverrideColor = null,
                                    string stunEffectId = null,
                                    float stunDuration = 0f,
                                    Color? stunOverrideColor = null,
                                    bool triggerOnHitEffect = true)
        {
            Owner = owner;
            DamageMultiplier = damageMultiplier;
            Speed = speed;
            MaxDistance = maxDistance;
            HitRadius = hitRadius;
            DotEffectId = dotEffectId;
            DotDamageMultiplier = dotDamageMultiplier;
            DotTickInterval = dotTickInterval;
            DotDuration = dotDuration;
            DisableEffectId = disableEffectId;
            DisableChance = disableChance;
            DisableDuration = disableDuration;
            DisableOverrideColor = disableOverrideColor;
            StunEffectId = stunEffectId;
            StunDuration = stunDuration;
            StunOverrideColor = stunOverrideColor;
            TriggerOnHitEffect = triggerOnHitEffect;
        }
    }
}
