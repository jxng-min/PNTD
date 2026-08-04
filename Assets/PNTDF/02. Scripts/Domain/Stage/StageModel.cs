using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class StageModel
    {
        public StageDomain Domain { get; }
        public StageCompositor Compositor { get; }

        private bool _isStageEnded;

        public StageModel(StageDomain domain, StageCompositor compositor)
        {
            Domain = domain;
            Compositor = compositor;
        }

        public void Initialize(EnemyFactory enemyFactory, StageContext stageContext, int stage)
        {
            _isStageEnded = false;
            
            Compositor.BindEvents();
            Domain.StageSystem.OnStageCleared += HandleOnStageEnded;
            
            Domain.Initialize(enemyFactory, stageContext);
            Compositor.Initialize(stageContext, stage);
        }

        public void StartStage()
        {
            Domain.StageSystem.StartStage();
        }

        public void Tick(float deltaTime)
        {
            Domain.StageSystem.Tick(deltaTime);
            Domain.WaveSystem.Tick(deltaTime);
        }

        public IEnumerator WaitUntilStageEnd()
        {
            yield return new WaitUntil(() => _isStageEnded);
        }

        public void Dispose()
        {
            Domain.StageSystem.OnStageCleared -= HandleOnStageEnded;
            Compositor.ReleaseEvents();
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

        private void HandleOnStageEnded()
        {
            _isStageEnded = true;
        }
    }
}
