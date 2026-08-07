using JxModule;

namespace PNTD
{
    public class StageCompositor
    {
        private readonly StageDomain _stageDomain;
        private readonly RStageContext _runtimeStageContext;
        private readonly ProgressView _progressView;
        private readonly FlowPresenter _flowPresenter;
        private readonly StageDeployAction _deployAction;
        private readonly JxCameraShaker _cameraShaker;
        private readonly TimeSlowEffect _timeSlowEffect;

        public StageCompositor(StageDomain stageDomain,
                               RStageContext runtimeStageContext,
                               ProgressView progressView,
                               FlowPresenter flowPresenter,
                               StageDeployAction deployAction,
                               JxCameraShaker cameraShaker,
                               TimeSlowEffect timeSlowEffect)
        {
            _stageDomain = stageDomain;
            _runtimeStageContext = runtimeStageContext;
            _progressView = progressView;
            _flowPresenter = flowPresenter;
            _deployAction = deployAction;
            _cameraShaker = cameraShaker;
            _timeSlowEffect = timeSlowEffect;
        }

        public void Initialize(StageContext stageContext, int stage)
        {
            var maxWave = stageContext?.Waves?.Count ?? 0;
            _progressView?.Initialize(_runtimeStageContext);
            _runtimeStageContext.Initialize(stage, stageContext?.MaxLife ?? 0, maxWave);
        }
        
        public void BindEvents()
        {
            BindStageEvents();
            BindWaveEvents();
            BindDeployEvents();
        }

        public void ReleaseEvents()
        {
            ReleaseStageEvents();
            ReleaseWaveEvents();
            ReleaseDeployEvents();
        }
        
#region Event Bindings
        private void HandleOnWaveBegin(int waveIndex, WaveContext waveContext)
        {
            _runtimeStageContext.UpdateWave(waveIndex + 1);
            _stageDomain.WaveSystem.StartWave(waveIndex, waveContext);
        }

        private void HandleOnDestinationReached(Enemy enemy)
        {
            _runtimeStageContext.UpdateLife(-1);
            _cameraShaker.ShakePosition(0.25f, 0.15f, 20);
            _timeSlowEffect.PlayReachEffect();
        }

        private void HandleOnWaveEnd(int waveIndex, WaveContext waveContext, float endDelay)
        {
            if (_flowPresenter != null)
            {
                _flowPresenter.StartCoroutine(_flowPresenter.Ready(endDelay));
            }
        }

        private void HandleOnEnemyDied(Enemy enemy)
        {
            _cameraShaker.ShakePosition(0.25f, 0.15f, 20);
        }

        private void BindStageEvents()
        {
            _stageDomain.StageSystem.OnWaveBegin += HandleOnWaveBegin;
            _stageDomain.StageSystem.OnWaveEnd += HandleOnWaveEnd;
        }

        private void ReleaseStageEvents()
        {
            _stageDomain.StageSystem.OnWaveBegin -= HandleOnWaveBegin;
            _stageDomain.StageSystem.OnWaveEnd -= HandleOnWaveEnd;
        }

        private void BindWaveEvents()
        {
            _stageDomain.WaveSystem.OnWaveEnd += _stageDomain.StageSystem.HandleOnWaveEnd;
            _stageDomain.WaveSystem.OnDestinationReached += HandleOnDestinationReached;
            _stageDomain.WaveSystem.OnEnemyDied += _stageDomain.PlunderSystem.HandleEnemyKilled;
            _stageDomain.WaveSystem.OnEnemyDied += HandleOnEnemyDied;
        }

        private void ReleaseWaveEvents()
        {
            _stageDomain.WaveSystem.OnWaveEnd -= _stageDomain.StageSystem.HandleOnWaveEnd;
            _stageDomain.WaveSystem.OnDestinationReached -= HandleOnDestinationReached;
            _stageDomain.WaveSystem.OnEnemyDied -= _stageDomain.PlunderSystem.HandleEnemyKilled;
            _stageDomain.WaveSystem.OnEnemyDied -= HandleOnEnemyDied;
        }

        private void BindDeployEvents()
        {
            _stageDomain.DeploySystem.OnEnterDeployMode += _stageDomain.DeployPreviewSystem.HandleOnEnterDeployMode;
            _stageDomain.DeploySystem.OnExitDeployMode += _stageDomain.DeployPreviewSystem.HandleOnExitDeployMode;
            _stageDomain.DeploySystem.OnDeployRequested += HandleOnDeployRequested;
        }

        private void ReleaseDeployEvents()
        {
            _stageDomain.DeploySystem.OnEnterDeployMode -= _stageDomain.DeployPreviewSystem.HandleOnEnterDeployMode;
            _stageDomain.DeploySystem.OnExitDeployMode -= _stageDomain.DeployPreviewSystem.HandleOnExitDeployMode;
            _stageDomain.DeploySystem.OnDeployRequested -= HandleOnDeployRequested;
        }

        private void HandleOnDeployRequested(DeployContext deployContext, UnityEngine.Vector3Int cellPosition)
        {
            _deployAction.TryDeploy(deployContext, cellPosition);
        }
#endregion
    }
}
