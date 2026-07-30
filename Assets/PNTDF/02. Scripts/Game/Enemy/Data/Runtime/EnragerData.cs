using UnityEngine;

namespace PNTD
{
    public class EnragerData : EnemyAbilityData
    {
        public float Radius { get; }
        public float MoveSpeedMultiplier { get; }
        public float BoostedDuration { get; }
        public float RecoverDuration { get; }
        public Color OverrideColor { get; }
        
        public EnragerData(string abilityID, 
                           float radius, 
                           float moveSpeedMultiplier, 
                           float boostedDuration, 
                           float recoverDuration, 
                           Color overrideColor) 
            : base(abilityID)
        {
            Radius = radius;
            MoveSpeedMultiplier = moveSpeedMultiplier;
            BoostedDuration = boostedDuration;
            RecoverDuration = recoverDuration;
            OverrideColor = overrideColor;
        }
    }
}