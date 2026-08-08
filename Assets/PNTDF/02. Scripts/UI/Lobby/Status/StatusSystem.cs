using System;
using UnityEngine;

namespace PNTD
{
    public class StatusSystem
    {
        private const int InitialStage = 1;
        private const int InitialGold = 5;
        private const int InitialHeroCountLimit = 7;
        
        private int _currentStage = InitialStage;
        private int _currentGold = InitialGold;
        private int _heroCountLimit = InitialHeroCountLimit;

        public event Action<int> OnUpdateGold;
        public event Action<int> OnUpdateStage;
        public event Action<int> OnUpdateHeroCountLimit;

        public int Stage => _currentStage;
        public int Gold => _currentGold;
        public int HeroCountLimit => _heroCountLimit;
        public int Interest => Mathf.Clamp(Gold / 5, 0, 5);

        public void Initialize()
        {
            OnUpdateStage?.Invoke(_currentStage);
            OnUpdateGold?.Invoke(_currentGold);
            OnUpdateHeroCountLimit?.Invoke(_heroCountLimit);
        }
        
        public void Reset()
        {
            _currentStage = InitialStage;
            _currentGold = InitialGold;
            _heroCountLimit = InitialHeroCountLimit;
            
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
            _heroCountLimit += amount;
            _heroCountLimit = Mathf.Clamp(_heroCountLimit, 0, 10);
            OnUpdateHeroCountLimit?.Invoke(_heroCountLimit);
        }
    }
}
