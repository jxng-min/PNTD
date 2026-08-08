using UnityEngine;

namespace PNTD
{
    public class EnemyHexEffect : HeroEffect
    {
        private readonly object _source;
        private readonly float _duration;
        private readonly float _attackDamagePenalty;
        private readonly float _attackSpeedPenalty;

        private float _remainingDuration;

        public object Source => _source;

        public EnemyHexEffect(object source,
                              float duration,
                              float attackDamagePenalty,
                              float attackSpeedPenalty)
        {
            _source = source;
            _duration = Mathf.Max(0f, duration);
            _attackDamagePenalty = Mathf.Clamp01(attackDamagePenalty);
            _attackSpeedPenalty = Mathf.Max(0f, attackSpeedPenalty);
            _remainingDuration = _duration;
        }

        public void Refresh()
        {
            _remainingDuration = _duration;
            IsFinished = false;
        }

        protected override void OnApply(Hero hero)
        {
            _remainingDuration = _duration;

            var attackMultiplier = Mathf.Max(0f, 1f - _attackDamagePenalty);
            var cooldownMultiplier = 1f + _attackSpeedPenalty;

            hero?.Stat?.AddModifier(this, EHeroStat.PhysicalAttackPower, EHeroStatModifierOperation.Multiply, attackMultiplier);
            hero?.Stat?.AddModifier(this, EHeroStat.MagicAttackPower, EHeroStatModifierOperation.Multiply, attackMultiplier);

            if (hero?.Skill is StarbornSkill starbornSkill)
            {
                starbornSkill.SetHexOrbitSpeedMultiplier(1f / cooldownMultiplier);
            }
            else
            {
                hero?.Stat?.AddModifier(this, EHeroStat.AttackCooldown, EHeroStatModifierOperation.Multiply, cooldownMultiplier);
            }
        }

        public override void Tick(Hero hero, float deltaTime)
        {
            if (IsFinished)
            {
                return;
            }

            _remainingDuration -= Mathf.Max(0f, deltaTime);
            if (_remainingDuration <= 0f)
            {
                IsFinished = true;
            }
        }

        public override void Release(Hero hero)
        {
            hero?.Stat?.RemoveModifiersFrom(this);
            if (hero?.Skill is StarbornSkill starbornSkill)
            {
                starbornSkill.SetHexOrbitSpeedMultiplier(1f);
            }
        }
    }
}
