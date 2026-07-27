using System;
using UnityEngine;

namespace PNTD
{
    public class StatusSystem
    {
        private int _currentStage = 1;
        private int _currentGold = 999;
        private int _heroCountLimit = 7;

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
        }
    }
}