using System;
using UnityEngine;

namespace PNTD
{
    public enum EClericAuraType
    {
        None = 0,
        AdaptivePenetration = 1,
        AttackPower = 2,
        AttackTempo = 3,
    }

    [Serializable]
    public class ClericSanctuaryData
    {
        public float radius = 1.75f;
        public EClericAuraType auraType;
        public float[] levelValues;
        public float tickInterval;
        public float[] judgedDamageTakenBonusValues;

        public float GetLevelValue(int level)
        {
            if (levelValues == null || levelValues.Length == 0)
            {
                return 0f;
            }

            var index = Mathf.Clamp(level, 1, levelValues.Length) - 1;
            return levelValues[index];
        }

        public float GetJudgedDamageTakenBonus(int level)
        {
            if (judgedDamageTakenBonusValues == null || judgedDamageTakenBonusValues.Length == 0)
            {
                return 0f;
            }

            var index = Mathf.Clamp(level, 1, judgedDamageTakenBonusValues.Length) - 1;
            return judgedDamageTakenBonusValues[index];
        }
    }
}
