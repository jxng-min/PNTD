using System;
using JxModule.DataTable;

namespace PNTD
{
    public class MapLoadSystem
    {
        private readonly DataTable _mapTable;

        public event Action<MapDataTableRow> OnLoadMapRequested;
        public event Action<StageMap> OnUnloadMapRequested;
        
        public StageMap CurrentMap { get; private set; }

        public MapLoadSystem(DataTable mapTable)
        {
            _mapTable = mapTable;
        }

        public void RequestLoadMap(StageContext stageContext)
        {
            if (stageContext == null)
            {
                return;
            }
            
            var mapDataTableRow = _mapTable.Find<MapDataTableRow>(row => row.isEnable && row.rowID == stageContext.MapId);
            if (mapDataTableRow == null)
            {
                return;
            }

            RequestUnloadMap();
            OnLoadMapRequested?.Invoke(mapDataTableRow);
        }

        public void RequestUnloadMap()
        {
            if (CurrentMap == null)
            {
                return;
            }
            
            OnUnloadMapRequested?.Invoke(CurrentMap);
            CurrentMap = null;
        }

        public void SetMap(StageMap map)
        {
            CurrentMap = map;
        }
    }
}