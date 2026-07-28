using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class HeroDataTableRow : DataTableRowBase
    {
        public string displayName;
        public Color color;
        public ETier tier;
        public ESynergy synergy;
        public int cost;
    }
}