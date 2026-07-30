using UnityEngine;

namespace PNTD
{
    public class EnemyDoTEffect : EnemyStatusEffect
    {
        private readonly float _damagePerTick;
        private readonly float _tickInterval;
        private readonly EAttack _attackType;
        private readonly float _flatPenetration;
        private readonly float _percentPenetration;
        
        private float _tickElapsedTime;
        
        public override string EffectID { get; }
        public override EEffectCategory Category => EEffectCategory.Debuff;
        public override EStackPolicy StackPolicy => EStackPolicy.RefreshDuration;

        public override float Strength => _damagePerTick;

        public EnemyDoTEffect(string effectId, 
                              float damagePerTick, 
                              float tickInterval, 
                              float duration,
                              float flatPenetration,
                              float percentPenetration,
                              EAttack attackType, 
                              Color? overrideColor = null)
            : base(duration, overrideColor)
        {
            EffectID = effectId;
            _damagePerTick = damagePerTick;
            _tickInterval = tickInterval;
            _attackType = attackType;
            _flatPenetration = flatPenetration;
            _percentPenetration = percentPenetration;
        }

        protected override void Tick(float deltaTime)
        {
            _tickElapsedTime += deltaTime;

            while (_tickElapsedTime >= _tickInterval)
            {
                _tickElapsedTime -= _tickInterval;
                ApplyDamage();
            }
        }

        private void ApplyDamage()
        {
            if (Owner == null)
            {
                return;
            }
            
            // TODO: EnemyHealth를 구현한 후 붙여야 함.
            var context = new DamageContext(_damagePerTick, _attackType, _flatPenetration, _percentPenetration);
            Owner.Health.TakeDamage(context);
        }
    }
}