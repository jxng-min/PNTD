namespace PNTD
{
    public class StageCompositor
    {
        private readonly StageDomain _stageDomain;

        public StageCompositor(StageDomain stageDomain)
        {
            _stageDomain = stageDomain;
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
        private void BindStageEvents()
        {
            _stageDomain.StageSystem.OnWaveBegin += _stageDomain.WaveSystem.StartWave;
        }

        private void ReleaseStageEvents()
        {
            _stageDomain.StageSystem.OnWaveBegin -= _stageDomain.WaveSystem.StartWave;
        }

        private void BindWaveEvents()
        {
            _stageDomain.WaveSystem.OnWaveEnd += _stageDomain.StageSystem.HandleOnWaveEnd;
        }

        private void ReleaseWaveEvents()
        {
            _stageDomain.WaveSystem.OnWaveEnd -= _stageDomain.StageSystem.HandleOnWaveEnd;
        }
#endregion
    }
}