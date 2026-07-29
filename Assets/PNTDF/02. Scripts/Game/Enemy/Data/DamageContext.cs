namespace PNTD
{
    public class DamageContext
    {
        public float Damage { get; }
        public EAttack AttackType { get; }
        public float FlatPenetration { get; }
        public float PercentPenetration { get; }

        public DamageContext(float damage, 
                             EAttack attackType, 
                             float flatPenetration = 0f,
                             float percentPenetration = 0f)
        {
            Damage = damage;
            AttackType = attackType;
            FlatPenetration = flatPenetration;
            PercentPenetration = percentPenetration;
        }
    }
}