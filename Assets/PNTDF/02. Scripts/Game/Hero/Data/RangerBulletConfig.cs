namespace PNTD
{
    public readonly struct RangerBulletConfig
    {
        public readonly Hero Owner;
        public readonly float DamageMultiplier;
        public readonly float Speed;
        public readonly float MaxDistance;
        public readonly float HitRadius;
        public readonly int MaxPierceCount;
        public readonly float VisualScale;
        public readonly bool TriggerOnHitEffect;

        public RangerBulletConfig(Hero owner,
                                  float damageMultiplier,
                                  float speed,
                                  float maxDistance,
                                  float hitRadius,
                                  int maxPierceCount,
                                  float visualScale = 1f,
                                  bool triggerOnHitEffect = true)
        {
            Owner = owner;
            DamageMultiplier = damageMultiplier;
            Speed = speed;
            MaxDistance = maxDistance;
            HitRadius = hitRadius;
            MaxPierceCount = maxPierceCount;
            VisualScale = visualScale;
            TriggerOnHitEffect = triggerOnHitEffect;
        }
    }
}
