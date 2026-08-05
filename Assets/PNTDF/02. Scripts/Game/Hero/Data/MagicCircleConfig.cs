namespace PNTD
{
    public enum EMagicCircleMode
    {
        DamageOnce = 0,
        AreaDamage = 1,
        Pull = 2,
    }
    
    public readonly struct MagicCircleConfig
    {
        public readonly Hero Owner;
        public readonly EMagicCircleMode Mode;
        public readonly float DamageMultiplier;
        public readonly float TickDamageMultiplier;
        public readonly float TickInterval;
        public readonly float PullDistance;
        public readonly float Radius;
        public readonly float ActivationDelay;
        public readonly float Duration;
        public readonly float RotationSpeed;
        public readonly float VisualScale;
        public readonly bool TriggerOnHitEffect;

        public MagicCircleConfig(Hero owner,
                                 float damageMultiplier,
                                 float radius,
                                 float activationDelay,
                                 float duration,
                                 float rotationSpeed,
                                 float visualScale = 1f,
                                 bool triggerOnHitEffect = true,
                                 EMagicCircleMode mode = EMagicCircleMode.DamageOnce,
                                 float tickDamageMultiplier = 0f,
                                 float tickInterval = 0f,
                                 float pullDistance = 0f)
        {
            Owner = owner;
            Mode = mode;
            DamageMultiplier = damageMultiplier;
            TickDamageMultiplier = tickDamageMultiplier;
            TickInterval = tickInterval;
            PullDistance = pullDistance;
            Radius = radius;
            ActivationDelay = activationDelay;
            Duration = duration;
            RotationSpeed = rotationSpeed;
            VisualScale = visualScale;
            TriggerOnHitEffect = triggerOnHitEffect;
        }
    }
}
