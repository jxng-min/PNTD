using UnityEngine;

namespace PNTD
{
    public class EnemyMagicResistanceEffect : EnemyStatusEffect
    {
        public override float MagicResistanceModifier { get; }
        public override string EffectID { get; }
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;

        public EnemyMagicResistanceEffect(string effectId, float modifier, float duration, Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
            MagicResistanceModifier = modifier;
        }
    }
}