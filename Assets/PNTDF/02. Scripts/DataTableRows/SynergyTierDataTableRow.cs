using JxModule.DataTable;

namespace PNTD
{
    public class SynergyTierDataTableRow : DataTableRowBase
    {
        public ESynergy synergy;
        public int threshold;
        public ESynergyEffect effect;
        public float primaryValue;
        public float secondaryValue;
        public string tierDescription;
    }
}