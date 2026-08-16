using System;
using UnityEngine;

namespace PNTD
{
    public class StatusSystem
    {
        private const int InitialStage = 1;
        private const int InitialGold = 5;
        private const int InitialHeroCountLimit = 7;
        private const int MaxHeroCountLimit = 10;
        
        private int _currentStage = InitialStage;
        private int _currentGold = InitialGold;
        private int _loopCount;

        public event Action<int> OnUpdateGold;
        public event Action<int> OnUpdateStage;
        public event Action<int> OnUpdateHeroCountLimit;

        public int Stage => _currentStage;
        public int Gold => _currentGold;
        public int LoopCount => _loopCount;
        public int HeroCountLimit => Mathf.Clamp(InitialHeroCountLimit + _loopCount, InitialHeroCountLimit, MaxHeroCountLimit);
        public int Interest => Mathf.Clamp(Gold / 5, 0, 5);

        public void Initialize()
        {
            OnUpdateStage?.Invoke(_currentStage);
            OnUpdateGold?.Invoke(_currentGold);
            OnUpdateHeroCountLimit?.Invoke(HeroCountLimit);
        }
        
        public void Reset()
        {
            _currentStage = InitialStage;
            _currentGold = InitialGold;
            _loopCount = 0;
            
            Initialize();
        }

        public void SetState(int stage, int gold, int loopCount = 0)
        {
            _currentStage = Mathf.Max(InitialStage, stage);
            _currentGold = Mathf.Clamp(gold, 0, int.MaxValue);
            var inferredLoopCount = StageLoopUtility.GetLoopCountFromStage(_currentStage);
            _loopCount = Mathf.Clamp(Mathf.Max(loopCount, inferredLoopCount), 0, MaxHeroCountLimit - InitialHeroCountLimit);
            
            Initialize();
        }

        public void UpdateGold(int amount)
        {
            _currentGold += amount;
            _currentGold = Mathf.Clamp(_currentGold, 0, int.MaxValue);
            OnUpdateGold?.Invoke(_currentGold);
        }

        public void UpdateStage(int amount)
        {
            _currentStage += amount;
            OnUpdateStage?.Invoke(_currentStage);
        }

        public void UpdateHeroCountLimit(int amount)
        {
            _loopCount = Mathf.Clamp(_loopCount + amount, 0, MaxHeroCountLimit - InitialHeroCountLimit);
            OnUpdateHeroCountLimit?.Invoke(HeroCountLimit);
        }

        public void AdvanceLoop()
        {
            UpdateHeroCountLimit(1);
        }
    }
}
