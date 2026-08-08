using JxModule.DataTable;

namespace PNTD
{
    public class SummonerDataTableRow : DataTableRowBase
    {
        public float firstCastDelay;
        public float coolDown;
        public int spawnCount;
        public int maxCasts;
        public string spawnedEnemyID;
        public float minSpawnOffset;
        public float maxSpawnOffset;
    }
}
