namespace PNTD
{
    public readonly struct RogueDaggerConfig
    {
        public readonly Hero Owner;
        public readonly float DamageMultiplier;
        public readonly float Speed;
        public readonly float MaxDistance;
        public readonly float HitRadius;
        public readonly float BleedDamageMultiplier;
        public readonly float BleedTickInterval;
        public readonly float BleedDuration;

        public RogueDaggerConfig(Hero owner,
                                 float damageMultiplier,
                                 float speed,
                                 float maxDistance,
                                 float hitRadius,
                                 float bleedDamageMultiplier,
                                 float bleedTickInterval,
                                 float bleedDuration)
        {
            Owner = owner;
            DamageMultiplier = damageMultiplier;
            Speed = speed;
            MaxDistance = maxDistance;
            HitRadius = hitRadius;
            BleedDamageMultiplier = bleedDamageMultiplier;
            BleedTickInterval = bleedTickInterval;
            BleedDuration = bleedDuration;
        }
    }
}
