using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class EnemyDataTableRow : DataTableRowBase
    {
        public Color color;
        public float maxHp;
        public float moveSpeed;
        public EEnemyType type;

        public float physicalDefense;
        public float magicResistance;
        public float slowResistance;

        public float rewardWeight = 1f;
        public int baseGoldReward = 5;
        public int lifeDamage = 1;
        public float spawnCostWeight = 1f;
        public bool countsForWaveClear = true;
        public bool grantsKillReward = true;
        public bool canDropPlunderGold = true;

        public string abilityID;
    }
}
