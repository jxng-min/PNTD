using System;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StageSystem
    {
        protected StageContext StageContext;

        private int _currentWaveIndex;
        private float _nextWaveDelayRemaining;
        private bool _waitingForNextWave;
        private bool _isStageRunning;

        public event Action<StageContext> OnStageBegin;
        public event Action<int, WaveContext> OnWaveBegin;
        public event Action<int, WaveContext, float> OnWaveEnd;
        public event Action OnStageCleared;
        
        public int RewardGold => StageContext?.RewardGold ?? 0;

        public void Initialize(StageContext stageContext)
        {
            if (stageContext == null)
            {
                DebugExtension.LogColor($"Stage System: Stage Context is null.", Color.red);
                return;
            }
            
            StageContext = stageContext;
            ResetProgress();
        }

        public void Tick(float deltaTime)
        {
            if (!_isStageRunning || !_waitingForNextWave)
            {
                return;
            }

            if (_nextWaveDelayRemaining > 0f)
            {
                _nextWaveDelayRemaining -= deltaTime;
                if (_nextWaveDelayRemaining > 0f)
                {
                    return;
                }
            }

            AdvanceToNextWave();
        }

        public void StartStage()
        {
            if (StageContext == null)
            {
                return;
            }

            ResetProgress();
            _isStageRunning = true;
            
            OnStageBegin?.Invoke(StageContext);
            StartCurrentWave();
        }

        public void HandleOnWaveEnd()
        {
            if (StageContext?.Waves == null || _currentWaveIndex >= StageContext.Waves.Count)
            {
                return;
            }
            
            var waveContext = StageContext.Waves[_currentWaveIndex];
            var endDelay = waveContext.EndDelay;
            
            OnWaveEnd?.Invoke(_currentWaveIndex, waveContext, endDelay);
            ScheduleNextWave(endDelay);
        }

        protected void StartCurrentWave()
        {
            if (StageContext?.Waves == null || 
                StageContext.Waves.Count == 0 ||
                _currentWaveIndex >= StageContext.Waves.Count)
            {
                OnStageCleared?.Invoke();
                _isStageRunning = false;
                return;
            }

            var waveContext = StageContext.Waves[_currentWaveIndex];
            OnWaveBegin?.Invoke(_currentWaveIndex, waveContext);
        }

        protected virtual void ScheduleNextWave(float endDelay)
        {
            _waitingForNextWave = true;
            _nextWaveDelayRemaining = endDelay;
        }

        protected void AdvanceToNextWave()
        {
            _waitingForNextWave = false;
            _nextWaveDelayRemaining = 0f;
            _currentWaveIndex++;
            StartCurrentWave();
        }

        private void ResetProgress()
        {
            _currentWaveIndex = 0;
            _nextWaveDelayRemaining = 0f;
            _waitingForNextWave = false;
            _isStageRunning = false;
        }
    }
}