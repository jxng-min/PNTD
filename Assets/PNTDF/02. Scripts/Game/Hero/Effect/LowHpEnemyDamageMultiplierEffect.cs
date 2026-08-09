using UnityEngine;

namespace PNTD
{
    public class LowHpEnemyDamageMultiplierEffect : HeroEffect
    {
        private readonly float _hpRateThreshold;
        private readonly float _damageMultiplier;

        public LowHpEnemyDamageMultiplierEffect(float hpRateThreshold, float damageMultiplier)
        {
            _hpRateThreshold = Mathf.Clamp01(hpRateThreshold);
            _damageMultiplier = Mathf.Max(0f, damageMultiplier);
        }

        public override float ModifyDamageToEnemy(Hero hero, Enemy enemy, float damage)
        {
            if (enemy == null ||
                enemy.Health == null ||
                enemy.Health.Rate > _hpRateThreshold)
            {
                return damage;
            }

            return damage * _damageMultiplier;
        }

        protected override void OnApply(Hero hero)
        {
        }
    }
}
