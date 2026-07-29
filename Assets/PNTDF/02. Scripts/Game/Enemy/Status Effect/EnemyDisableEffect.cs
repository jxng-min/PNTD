using UnityEngine;

namespace PNTD
{
    public class EnemyDisableEffect : EnemyStatusEffect
    {
        public override bool IsDisabled => true;
        public override string EffectID { get; }
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;

        public EnemyDisableEffect(string effectId, float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
        }
    }
}