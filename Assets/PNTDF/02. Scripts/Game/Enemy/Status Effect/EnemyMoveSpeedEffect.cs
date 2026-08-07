using UnityEngine;

namespace PNTD
{
    public class EnemyMoveSpeedEffect : EnemyStatusEffect
    {
        private readonly EStackPolicy _stackPolicy;
        
        public override float MoveSpeedMultiplier { get; }
        public override string EffectID { get; }
        public override EStackPolicy StackPolicy => _stackPolicy;
        
        public EnemyMoveSpeedEffect(string effectId, 
                                    float multiplier, 
                                    float duration, 
                                    EStackPolicy stackPolicy = EStackPolicy.KeepStrongest,
                                    Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
            MoveSpeedMultiplier = multiplier;
            _stackPolicy = stackPolicy;
        }
    }
}
