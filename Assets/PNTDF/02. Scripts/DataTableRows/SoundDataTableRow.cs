using System.Collections.Generic;
using JxModule.DataTable;

namespace PNTD
{
    public class SoundDataTableRow : DataTableRowBase
    {
        public ESound type;
        public List<string> soundAddresses;
        public int channelCount;
        public bool loop;
    }
}