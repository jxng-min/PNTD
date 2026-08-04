namespace PNTD
{
    public class StageCompositor
    {
        private readonly StageDomain _stageDomain;
        private readonly RStageContext _runtimeStageContext;
        private readonly ProgressView _progressView;
        private readonly FlowPresenter _flowPresenter;

        public StageCompositor(StageDomain stageDomain,
                               RStageContext runtimeStageContext,
                               ProgressView progressView,
                               FlowPresenter flowPresenter)
        {
            _stageDomain = stageDomain;
            _runtimeStageContext = runtimeStageContext;
            _progressView = progressView;
            _flowPresenter = flowPresenter;
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
        }

        public void ReleaseEvents()
        {
            ReleaseStageEvents();
            ReleaseWaveEvents();
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
        }

        private void HandleOnWaveEnd(int waveIndex, WaveContext waveContext, float endDelay)
        {
            if (_flowPresenter != null)
            {
                _flowPresenter.StartCoroutine(_flowPresenter.Ready(endDelay));
            }
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
        }

        private void ReleaseWaveEvents()
        {
            _stageDomain.WaveSystem.OnWaveEnd -= _stageDomain.StageSystem.HandleOnWaveEnd;
            _stageDomain.WaveSystem.OnDestinationReached -= HandleOnDestinationReached;
        }
#endregion
    }
}
