namespace PNTD
{
    public static class EnemyAbilityFactory
    {
        public static EnemyAbility Create(EnemyAbilityData abilityData)
        {
            return abilityData switch
            {
                EnragerData enragerData         => new EnragerAbility(enragerData),
                BlinkerData blinkerData         => new BlinkerAbility(blinkerData),
                HexerData hexerData             => new HexerAbility(hexerData),
                SummonerData summonerData       => new SummonerAbility(summonerData),
                _                               => null
            };
        }
    }
}
