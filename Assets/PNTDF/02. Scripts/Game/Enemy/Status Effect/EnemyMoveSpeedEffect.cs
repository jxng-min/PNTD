using UnityEngine;

namespace PNTD
{
    public class EnemyMoveSpeedEffect : EnemyStatusEffect
    {
        public override float MoveSpeedMultiplier { get; }
        public override string EffectID { get; }
        public override EStackPolicy StackPolicy => EStackPolicy.KeepStrongest;
        
        public EnemyMoveSpeedEffect(string effectId, float multiplier, float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
            MoveSpeedMultiplier = multiplier;
        }
    }
}