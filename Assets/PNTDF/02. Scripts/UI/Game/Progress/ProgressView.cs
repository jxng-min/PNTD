using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ProgressView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private ShadowableLabel lifeLabel;
        [SerializeField] private ShadowableLabel waveLabel;
        
        private RStageContext _runtimeStageContext;
        private bool _isInitialized;

        public void Initialize(RStageContext runtimeStageContext)
        {
            if (_isInitialized)
            {
                Release();
            }
            
            _runtimeStageContext = runtimeStageContext;
            if (_runtimeStageContext == null)
            {
                return;
            }

            _runtimeStageContext.OnUpdateLife += UpdateLifeLabelView;
            _runtimeStageContext.OnUpdateWave += UpdateWaveLabelView;
            _isInitialized = true;
        }
        
        private void UpdateLifeLabelView(int currentLife, int maxLife)
        {
            lifeLabel.SetText($"life:<size=14> </size><color=#32FF83><size=26>{currentLife}/{maxLife}</size></color>");
        }

        private void UpdateWaveLabelView(int currentWave, int maxWave)
        {
            waveLabel.SetText($"wave:<size=14> </size><color=#10EFFF><size=26>{currentWave - 1}/{maxWave - 1}</size></color>");
        }

        private void Release()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_runtimeStageContext != null)
            {
                _runtimeStageContext.OnUpdateLife -= UpdateLifeLabelView;
                _runtimeStageContext.OnUpdateWave -= UpdateWaveLabelView;
            }
            
            _runtimeStageContext = null;
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}