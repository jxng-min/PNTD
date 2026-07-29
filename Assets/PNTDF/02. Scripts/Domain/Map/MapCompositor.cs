using JxModule;
using UnityEngine;

namespace PNTD
{
    public class MapCompositor
    {
        private readonly MapDomain _mapDomain;
        private readonly MapFactory _mapFactory;

        public MapCompositor(MapDomain mapDomain, MapFactory mapFactory)
        {
            _mapDomain = mapDomain;
            _mapFactory = mapFactory;
        }

        public void BindEvents()
        {
            BindMapLoadEvents();
        }

        public void ReleaseEvents()
        {
            ReleaseMapLoadEvents();
        }
        
#region Event Handling
        private void HandleOnLoadMapRequested(MapDataTableRow mapDataTableRow)
        {
            if (mapDataTableRow == null || mapDataTableRow.prefab == null)
            {
                DebugExtension.LogColor($"Map Compositor: MapDataTableRow or Data is null.", Color.red);
                return;
            }
            
            var map = _mapFactory.Create(mapDataTableRow.prefab);
            if (map == null)
            {
                DebugExtension.LogColor($"Map Compositor: Failed to create map from prefab.", Color.red);
                return;
            }
            
            _mapDomain.MapLoadSystem.SetMap(map);
        }

        private void HandleOnUnloadMapRequested(StageMap stageMap)
        {
            _mapFactory.Release(stageMap);
        }
#endregion

#region Event Bindings
        private void BindMapLoadEvents()
        {
            _mapDomain.MapLoadSystem.OnLoadMapRequested += HandleOnLoadMapRequested;
            _mapDomain.MapLoadSystem.OnUnloadMapRequested += HandleOnUnloadMapRequested;
        }

        private void ReleaseMapLoadEvents()
        {
            _mapDomain.MapLoadSystem.OnLoadMapRequested -= HandleOnLoadMapRequested;
            _mapDomain.MapLoadSystem.OnUnloadMapRequested -= HandleOnUnloadMapRequested;
        }
#endregion
    }
}