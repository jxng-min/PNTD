namespace PNTD
{
    public class StageDomain
    {
        private readonly StageSystem _stageSystem;
        private readonly WaveSystem _waveSystem;
        private readonly StageVisibilitySystem _visibilitySystem;
        
        public StageSystem StageSystem => _stageSystem;
        public WaveSystem WaveSystem => _waveSystem;
        public StageVisibilitySystem VisibilitySystem => _visibilitySystem;

        public StageDomain(StageSystem stageSystem, WaveSystem waveSystem, StageVisibilitySystem visibilitySystem)
        {
            _stageSystem = stageSystem;
            _waveSystem = waveSystem;
            _visibilitySystem = visibilitySystem;
        }

        public void Initialize(EnemyFactory enemyFactory, StageContext stageContext)
        {
            _waveSystem.Initialize(enemyFactory);
            _stageSystem.Initialize(stageContext);
        }
    }
}
