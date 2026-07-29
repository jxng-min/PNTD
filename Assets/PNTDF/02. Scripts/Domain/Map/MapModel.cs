using JxModule;
using UnityEngine;

namespace PNTD
{
    public class MapModel
    {
        public MapDomain Domain { get; }
        public MapCompositor Compositor { get; }

        public MapModel(MapDomain domain, MapCompositor compositor)
        {
            Domain = domain;
            Compositor = compositor;
        }

        public void Initialize()
        {
            Compositor.BindEvents();
        }

        public MapContext LoadMap(string stageId)
        {
            var stageContext = Domain.BuildStageContext(stageId);
            if (stageContext == null)
            {
                return null;
            }

            var map = Domain.MapLoadSystem.CurrentMap;
            if (map == null)
            {
                DebugExtension.LogColor($"Map Model: Failed to load map. Stage ID: {stageId}", Color.red);
                return null;
            }
            
            return new MapContext(map, stageContext);
        }

        public void UnloadMap()
        {
            Domain.RequestUnloadMap();
        }

        public void Dispose()
        {
            Compositor.ReleaseEvents();
        }
    }
}