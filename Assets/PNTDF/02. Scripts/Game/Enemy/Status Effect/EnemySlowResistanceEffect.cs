using UnityEngine;

namespace PNTD
{
    public class EnemySlowResistanceEffect : EnemyStatusEffect
    {
        public override float SlowResistanceModifier { get; }
        public override string EffectID { get; }
        
        public EnemySlowResistanceEffect(string effectId, float modifier, float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
            SlowResistanceModifier = modifier;
        }
    }
}