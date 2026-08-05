using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class StageModel
    {
        public enum EStageResult
        {
            None,
            Clear,
            Over
        }

        public StageDomain Domain { get; }
        public StageCompositor Compositor { get; }

        private readonly RStageContext _runtimeStageContext;
        
        private bool _isStageEnded;
        private int _reachedStage;
        private EStageResult _stageResult = EStageResult.None;

        public EStageResult StageResult => _stageResult;
        public int ReachedStage => _reachedStage;
        public int RewardGold => Domain.StageSystem.RewardGold;
        public int BonusGold => _runtimeStageContext.Gold;

        public StageModel(StageDomain domain, StageCompositor compositor, RStageContext runtimeStageContext)
        {
            Domain = domain;
            Compositor = compositor;
            _runtimeStageContext = runtimeStageContext;
        }

        public void Initialize(EnemyFactory enemyFactory, StageContext stageContext, int stage)
        {
            _isStageEnded = false;
            _reachedStage = stage;
            _stageResult = EStageResult.None;
            
            Compositor.BindEvents();
            Domain.StageSystem.OnStageCleared += HandleOnStageCleared;
            _runtimeStageContext.OnStageOvered += HandleOnStageOvered;
            
            Domain.Initialize(enemyFactory, stageContext);
            Compositor.Initialize(stageContext, stage);
        }

        public void StartStage()
        {
            Domain.StageSystem.StartStage();
        }

        public void Tick(float deltaTime)
        {
            if (_isStageEnded)
            {
                return;
            }

            Domain.StageSystem.Tick(deltaTime);
            Domain.WaveSystem.Tick(deltaTime);
            Domain.DeployPreviewSystem.Tick();
        }

        public IEnumerator WaitUntilStageEnd()
        {
            yield return new WaitUntil(() => _isStageEnded);
        }

        public void Dispose()
        {
            Domain.StageSystem.OnStageCleared -= HandleOnStageCleared;
            _runtimeStageContext.OnStageOvered -= HandleOnStageOvered;
            Compositor.ReleaseEvents();
            Domain.DeployPreviewSystem.Dispose();
            Domain.WaveSystem.Dispose();
        }

        public void Show()
        {
            Domain.VisibilitySystem.Show();
        }

        public void Hide()
        {
            Domain.VisibilitySystem.Hide();
        }

        private void HandleOnStageCleared()
        {
            _stageResult = EStageResult.Clear;
            _isStageEnded = true;
        }

        private void HandleOnStageOvered(int reachedStage)
        {
            _reachedStage = reachedStage;
            _stageResult = EStageResult.Over;
            _isStageEnded = true;
            Domain.WaveSystem.StopWave();
        }
    }
}
