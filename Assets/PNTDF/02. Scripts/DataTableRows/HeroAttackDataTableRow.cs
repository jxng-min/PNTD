using JxModule.DataTable;

namespace PNTD
{
    public class HeroAttackDataTableRow : DataTableRowBase
    {
        public float physicalAttackPower;
        public float magicAttackPower;

        public float attackCooldown;
        public float attackRange;

        public float physicalFlatPenetration;
        public float physicalPercentPenetration;

        public float magicFlatPenetration;
        public float magicPercentPenetration;
    }
}