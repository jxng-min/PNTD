namespace PNTD
{
    public readonly struct DamageContext
    {
        public float Damage { get; }
        public EAttack AttackType { get; }
        public float FlatPenetration { get; }
        public float PercentPenetration { get; }
        public Hero SourceHero { get; }

        public DamageContext(float damage, 
                             EAttack attackType, 
                             float flatPenetration = 0f,
                             float percentPenetration = 0f,
                             Hero sourceHero = null)
        {
            Damage = damage;
            AttackType = attackType;
            FlatPenetration = flatPenetration;
            PercentPenetration = percentPenetration;
            SourceHero = sourceHero;
        }

        public DamageContext WithSource(Hero sourceHero)
        {
            return new DamageContext(Damage, AttackType, FlatPenetration, PercentPenetration, sourceHero);
        }
    }
}
