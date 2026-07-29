using JxModule;
using UnityEngine;

namespace PNTD
{
    public class MapDomain
    {
        public MapLoadSystem MapLoadSystem { get; }
        public StageBuildSystem StageBuildSystem { get; }

        public MapDomain(MapLoadSystem mapLoadSystem, StageBuildSystem stageBuildSystem)
        {
            MapLoadSystem = mapLoadSystem;
            StageBuildSystem = stageBuildSystem;
        }

        public StageContext BuildStageContext(string stageId)
        {
            var stageContext = StageBuildSystem.Build(stageId);
            if (stageContext == null)
            {
                DebugExtension.LogColor($"Map Domain: Failed to build Stage Context.", Color.red);
                return null;
            }
            
            MapLoadSystem.RequestLoadMap(stageContext);
            return stageContext;
        }

        public void RequestUnloadMap()
        {
            MapLoadSystem.RequestUnloadMap();
        }
    }
}