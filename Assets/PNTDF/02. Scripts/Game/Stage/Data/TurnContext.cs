namespace PNTD
{
    public class TurnContext
    {
        public string EnemyId { get; }
        public int SpawnCount { get; }
        public float StartTime { get; }
        public float SpawnInterval { get; }

        public TurnContext(string enemyId, int spawnCount, float startTime, float spawnInterval)
        {
            EnemyId = enemyId;
            SpawnCount = spawnCount;
            StartTime = startTime;
            SpawnInterval = spawnInterval;
        }
    }
}