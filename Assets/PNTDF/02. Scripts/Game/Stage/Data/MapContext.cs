namespace PNTD
{
    public class MapContext
    {
        public StageMap Map { get; }
        public StageContext StageContext { get; }

        public MapContext(StageMap map, StageContext stageContext)
        {
            Map = map;
            StageContext = stageContext;
        }
    }
}