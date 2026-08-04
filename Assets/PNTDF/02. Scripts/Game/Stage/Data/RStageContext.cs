using System;
using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class RStageContext
    {
        private int _currentStage;

        private int _currentLife;
        private int _currentGold;
        private int _currentWave;

        private int _maxLife;
        private int _maxWave;

        private HashSet<string> _usingParty = new();

        public event Action<int, int> OnUpdateLife;
        public event Action<int, int> OnUpdateWave;
        public event Action<int> OnStageOvered;
        public event Action<string, bool> OnUpdateHero;
        
        public int Gold => _currentGold;

        public void Initialize(int stage, int maxLife, int maxWave)
        {
            _usingParty.Clear();
            
            _currentStage = stage;
            _currentGold = 0;
            
            _maxLife = maxLife;
            _currentLife = _maxLife;
            
            _maxWave = maxWave;
            _currentWave = 1;
            
            OnUpdateLife?.Invoke(_currentLife, _maxLife);
            OnUpdateWave?.Invoke(_currentWave, _maxWave);
        }

        public void UpdateLife(int value)
        {
            _currentLife += value;
            _currentLife = Mathf.Clamp(_currentLife, 0, _maxLife);
            OnUpdateLife?.Invoke(_currentLife, _maxLife);

            if (_currentLife <= 0)
            {
                OnStageOvered?.Invoke(_currentStage);
            }
        }

        public void UpdateWave(int value)
        {
            _currentWave = Mathf.Clamp(value, 0, _maxWave);
            OnUpdateWave?.Invoke(_currentWave, _maxWave);
        }

        public void UpdateGold(int value)
        {
            _currentGold += value;
        }
        
        public void UpdateHero(string heroID, bool isUsing)
        {
            if (isUsing)
            {
                _usingParty.Add(heroID);
            }
            else
            {
                _usingParty.Remove(heroID);
            }
            
            OnUpdateHero?.Invoke(heroID, isUsing);
        }

    }
}