using UnityEngine;

namespace PNTD
{
    public class EnemyPhysicalDefenseEffect : EnemyStatusEffect
    {
        public override float PhysicalDefenseModifier { get; }
        public override string EffectID { get; }
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;

        public EnemyPhysicalDefenseEffect(string effectId, float modifier, float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
            PhysicalDefenseModifier = modifier;
        }
    }
}