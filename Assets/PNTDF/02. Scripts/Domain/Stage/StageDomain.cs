namespace PNTD
{
    public class StageDomain
    {
        private readonly StageSystem _stageSystem;
        private readonly WaveSystem _waveSystem;
        
        public StageSystem StageSystem => _stageSystem;
        public WaveSystem WaveSystem => _waveSystem;

        public StageDomain(StageSystem stageSystem, WaveSystem waveSystem)
        {
            _stageSystem = stageSystem;
            _waveSystem = waveSystem;
        }

        public void Initialize(EnemyFactory enemyFactory, StageContext stageContext)
        {
            _waveSystem.Initialize(enemyFactory);
            _stageSystem.Initialize(stageContext);
        }
    }
}