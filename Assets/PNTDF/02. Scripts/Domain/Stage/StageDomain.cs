namespace PNTD
{
    public class StageDomain
    {
        private readonly StageSystem _stageSystem;
        private readonly WaveSystem _waveSystem;
        private readonly StageVisibilitySystem _visibilitySystem;
        private readonly BoardSystem _boardSystem;
        private readonly DeploySystem _deploySystem;
        private readonly HeroFactory _heroFactory;
        private readonly DeployPreviewSystem _deployPreviewSystem;
        private readonly HeroMoveSystem _heroMoveSystem;
        private readonly ClericSanctuarySystem _clericSanctuarySystem;
        
        public StageSystem StageSystem => _stageSystem;
        public WaveSystem WaveSystem => _waveSystem;
        public StageVisibilitySystem VisibilitySystem => _visibilitySystem;
        public BoardSystem BoardSystem => _boardSystem;
        public DeploySystem DeploySystem => _deploySystem;
        public HeroFactory HeroFactory => _heroFactory;
        public DeployPreviewSystem DeployPreviewSystem => _deployPreviewSystem;
        public HeroMoveSystem HeroMoveSystem => _heroMoveSystem;
        public ClericSanctuarySystem ClericSanctuarySystem => _clericSanctuarySystem;

        public StageDomain(StageSystem stageSystem,
                           WaveSystem waveSystem,
                           StageVisibilitySystem visibilitySystem,
                           BoardSystem boardSystem,
                           DeploySystem deploySystem,
                           HeroFactory heroFactory,
                           DeployPreviewSystem deployPreviewSystem,
                           HeroMoveSystem heroMoveSystem,
                           ClericSanctuarySystem clericSanctuarySystem)
        {
            _stageSystem = stageSystem;
            _waveSystem = waveSystem;
            _visibilitySystem = visibilitySystem;
            _boardSystem = boardSystem;
            _deploySystem = deploySystem;
            _heroFactory = heroFactory;
            _deployPreviewSystem = deployPreviewSystem;
            _heroMoveSystem = heroMoveSystem;
            _clericSanctuarySystem = clericSanctuarySystem;
        }

        public void Initialize(EnemyFactory enemyFactory, StageContext stageContext)
        {
            _boardSystem.Initialize();
            _waveSystem.Initialize(enemyFactory);
            _stageSystem.Initialize(stageContext);
        }
    }
}
