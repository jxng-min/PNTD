using JxModule.DataTable;

namespace PNTD
{
    public class EnemyAbilityDataTableRow : DataTableRowBase
    {
        public EEnemyAbility ability;
        public float coolDown;
        public EEnemyTriggerType trigger;
    }
}