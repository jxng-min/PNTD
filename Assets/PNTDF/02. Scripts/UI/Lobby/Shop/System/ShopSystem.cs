using System;

namespace PNTD
{
    public class ShopSystem
    {
        private const int InitialLevel = 1;
        private const int InitialExp = 0;
        private const int InitialRerollCost = 2;
        private const int MaxLevel = 5;
        private const int MaxExp = 7;

        public event Action<int, int> OnUpdateLevel;
        public event Action OnRequestShopRoll;
        
        public int Level { get; private set; } = InitialLevel;
        public int Exp { get; private set; }
        public int LevelCost => Level + 1;
        public int RerollCost { get; private set; } = InitialRerollCost;
        public bool IsMaxLevel => Level >= MaxLevel;
        public bool IsLock { get; private set; }

        public void Initialize()
        {
            Roll();
        }
        
        public void Reset()
        {
            Level = InitialLevel;
            Exp = InitialExp;
            RerollCost = InitialRerollCost;
            IsLock = false;
            
            OnUpdateLevel?.Invoke(Level, Exp);
            Roll();
        }

        public void UpdateLock(bool isOn)
        {
            IsLock = isOn;
        }

        public void UpdateLevel()
        {
            if (Level >= MaxLevel)
            {
                return;
            }

            if (Exp + 1 >= MaxExp)
            {
                SoundManager.Instance.PlaySFX("SFX_LevelUp");
                Level += 1;
                Exp = 0;
            }
            else
            {
                Exp += 1;
            }
            
            OnUpdateLevel?.Invoke(Level, Exp);
        }

        private void Roll()
        {
            OnRequestShopRoll?.Invoke();
        }
    }
}
