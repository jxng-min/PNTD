namespace PNTD
{
    public static class EnemyAbilityFactory
    {
        public static EnemyAbility Create(EnemyAbilityData abilityData)
        {
            return abilityData switch
            {
                _ => null
            };
        }
    }
}