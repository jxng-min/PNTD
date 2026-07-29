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

        public string abilityID;
    }
}