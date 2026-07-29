using JxModule;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class MapRunner : MonoBehaviour
    {
        [SerializeField] private MapFactory mapFactory;

        private DataTable _mapDataTable;
        private DataTable _stageDataTable;
        private DataTable _waveDataTable;
        private DataTable _turnDataTable;
        private DataTable _enemyDataTable;

        private MapModel _model;
        private bool _isInitialized;

        private void Awake()
        {
            _mapDataTable = DataTableManager.FindTable<MapDataTableRow>("DT_Map");
            _stageDataTable = DataTableManager.FindTable<StageDataTableRow>("DT_Stage");
            _waveDataTable = DataTableManager.FindTable<WaveDataTableRow>("DT_Wave");
            _turnDataTable = DataTableManager.FindTable<TurnDataTableRow>("DT_Turn");
            _enemyDataTable = DataTableManager.FindTable<EnemyDataTableRow>("DT_Enemy");
        }

        private void Start()
        {
            if (mapFactory == null)
            {
                DebugExtension.LogColor($"Map Runner: Map Factory is null.", Color.red);
                enabled = false;
                return;
            }
            
            var mapLoadSystem = new MapLoadSystem(_mapDataTable);
            var stageBuildSystem = new StageBuildSystem(_stageDataTable, _waveDataTable, _turnDataTable, _enemyDataTable);
            
            var mapDomain = new MapDomain(mapLoadSystem, stageBuildSystem);
            var mapCompositor = new MapCompositor(mapDomain, mapFactory);

            _model = new MapModel(mapDomain, mapCompositor);
            _model.Initialize();
            _isInitialized = true;
        }

        public MapContext LoadMap(string stageId)
        {
            if (!_isInitialized || _model == null)
            {
                DebugExtension.LogColor($"Map Runner: LoadMap was called before initialization.", Color.red);
                return null;
            }
            
            return _model.LoadMap(stageId);
        }

        public void UnloadMap()
        {
            if (!_isInitialized || _model == null)
            {
                DebugExtension.LogColor($"Map Runner: UnloadMap was called before initialization.", Color.red);
                return;
            }
            
            _model.UnloadMap();
        }
        
        private void OnDestroy()
        {
            _model?.Dispose();
        }
    }
}