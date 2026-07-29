using UnityEngine;

namespace PNTD
{
    public class EnemyInvincibleEffect : EnemyStatusEffect
    {
        public override string EffectID => "Invincible";
        public override EEffectCategory Category => EEffectCategory.Buff;
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;
        public override bool IsInvincible => true;
        
        public EnemyInvincibleEffect(float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {}
    }
}