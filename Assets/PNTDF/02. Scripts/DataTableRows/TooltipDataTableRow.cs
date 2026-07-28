using JxModule.DataTable;

namespace PNTD
{
    public class TooltipDataTableRow : DataTableRowBase
    {
        public ETooltipLayout layout;

        public string headerText;
        public string tagText;
        public string bodyText;

        public string additionalHeaderText;
        public string additionalTagText;
        public string additionalBodyText;
    }
}