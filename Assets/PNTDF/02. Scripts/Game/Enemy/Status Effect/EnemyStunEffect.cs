using UnityEngine;

namespace PNTD
{
    public class EnemyStunEffect : EnemyStatusEffect
    {
        public override bool IsStun => true;
        public override string EffectID { get; }
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;

        public EnemyStunEffect(string effectId, float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
        }
    }
}