namespace PNTD
{
    public class SummonerData : EnemyAbilityData
    {
        public float FirstCastDelay { get; }
        public float CoolDown { get; }
        public int SpawnCount { get; }
        public int MaxCasts { get; }
        public string SpawnedEnemyID { get; }
        public float MinSpawnOffset { get; }
        public float MaxSpawnOffset { get; }

        public SummonerData(string abilityID,
                            float firstCastDelay,
                            float coolDown,
                            int spawnCount,
                            int maxCasts,
                            string spawnedEnemyID,
                            float minSpawnOffset,
                            float maxSpawnOffset)
            : base(abilityID)
        {
            FirstCastDelay = firstCastDelay;
            CoolDown = coolDown;
            SpawnCount = spawnCount;
            MaxCasts = maxCasts;
            SpawnedEnemyID = spawnedEnemyID;
            MinSpawnOffset = minSpawnOffset;
            MaxSpawnOffset = maxSpawnOffset;
        }
    }
}
