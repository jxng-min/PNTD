using UnityEngine;

namespace PNTD
{
    public class EnemyImmuneEffect : EnemyStatusEffect
    {
        public override string EffectID => "StatusEffectImmune";
        public override EEffectCategory Category => EEffectCategory.Buff;
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;
        public override bool IsEffectImmune => true;
        
        public EnemyImmuneEffect(float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {}
    }
}