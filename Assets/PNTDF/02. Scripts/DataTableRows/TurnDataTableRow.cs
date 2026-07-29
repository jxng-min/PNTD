using JxModule.DataTable;

namespace PNTD
{
    public class TurnDataTableRow : DataTableRowBase
    {
        public string waveID;
        public string enemyID;
        public int spawnCount;
        public float startTime;
        public float spawnInterval;
        public int order;
    }
}