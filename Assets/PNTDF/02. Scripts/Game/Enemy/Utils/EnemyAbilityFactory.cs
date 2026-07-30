namespace PNTD
{
    public static class EnemyAbilityFactory
    {
        public static EnemyAbility Create(EnemyAbilityData abilityData)
        {
            return abilityData switch
            {
                EnragerData enragerData         => new EnragerAbility(enragerData),
                _                               => null
            };
        }
    }
}