using UnityEngine;

namespace PNTD
{
    public class MageSynergySlowEffect : HeroEffect
    {
        private const string EffectId = "MageSynergySlow";
        
        private readonly float _moveSpeedMultiplier;
        private readonly float _duration;

        public MageSynergySlowEffect(float moveSpeedMultiplier, float duration)
        {
            _moveSpeedMultiplier = Mathf.Clamp(moveSpeedMultiplier, 0f, 1f);
            _duration = Mathf.Max(0f, duration);
        }

        public override void OnAffectEnemy(Hero hero, Enemy enemy)
        {
            if (enemy == null || enemy.Status == null)
            {
                return;
            }

            enemy.Status.AddMoveSpeedEffect(EffectId,
                                            _moveSpeedMultiplier,
                                            _duration,
                                            stackPolicy: EStackPolicy.RefreshDuration);
        }

        protected override void OnApply(Hero hero) {}
    }
}
