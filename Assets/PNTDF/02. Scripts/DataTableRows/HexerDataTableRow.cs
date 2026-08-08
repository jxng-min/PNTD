using JxModule.DataTable;

namespace PNTD
{
    public class HexerDataTableRow : DataTableRowBase
    {
        public float firstCastDelay;
        public float coolDown;
        public float castRange;
        public float duration;
        public float attackDamagePenalty;
        public float attackSpeedPenalty;
        public int maxActiveHexPerHexer = 1;
    }
}
