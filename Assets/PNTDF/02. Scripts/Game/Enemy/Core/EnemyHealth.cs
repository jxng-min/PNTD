using System;
using UnityEngine;

namespace PNTD
{
    public class EnemyHealth : MonoBehaviour
    {
        private const float DefenseCoefficient = 100f;

        private EnemyStatus _status;

        public event Action<float, float> OnEnemyDamaged;
        public event Action OnEnemyDied;
        
        public float MaxHp { get; private set; }
        public float CurrentHp { get; private set; }
        public float Rate => MaxHp > 0f ? CurrentHp / MaxHp : 0f;
        
        public bool IsDead => CurrentHp <= 0f;

        public void Initialize(EnemyStatus enemyStatus, float maxHp)
        {
            _status = enemyStatus;

            MaxHp = Mathf.Max(1f, maxHp);
            CurrentHp = MaxHp;
        }

        public bool TakeDamage(in DamageContext damageContext)
        {
            if (IsDead || damageContext.Damage <= 0f)
            {
                return false;
            }

            if (_status != null && _status.IsInvincible)
            {
                return false;
            }

            var finalDamage = CalculateDamage(damageContext);
            if (finalDamage <= 0f)
            {
                return false;
            }
            
            CurrentHp = Mathf.Max(0f, CurrentHp - finalDamage);
            OnEnemyDamaged?.Invoke(CurrentHp, MaxHp);

            if (CurrentHp <= 0f)
            {
                OnEnemyDied?.Invoke();
            }
            
            SoundManager.Instance.PlaySFX("SFX_Damage");

            return true;
        }

        private float CalculateDamage(in DamageContext damageContext)
        {
            if (damageContext.AttackType == EAttack.True)
            {
                return Mathf.Max(0f, damageContext.Damage);
            }
            
            var resistance = GetResistance(damageContext.AttackType);
            var effectiveResistance = CalculateEffectiveResistance(resistance,
                                                                   damageContext.FlatPenetration,
                                                                   damageContext.PercentPenetration);
            
            var damage = ApplyResistance(damageContext.Damage, effectiveResistance);
            return ApplyJudgedMultiplier(damage, damageContext.SourceHero);
        }

        private float ApplyJudgedMultiplier(float damage, Hero sourceHero)
        {
            if (_status == null ||
                !_status.IsJudgedByCrusader ||
                sourceHero == null ||
                !ClericSanctuaryRegistry.HasAlliedAura(sourceHero))
            {
                return damage;
            }

            return damage * (1f + _status.CrusaderJudgedDamageTakenBonus);
        }

        private float GetResistance(EAttack attackType)
        {
            if (_status == null)
            {
                return 0f;
            }
            
            return attackType switch
            {
                EAttack.Physical    => _status.FinalPhysicalDefense,
                EAttack.Magic       => _status.FinalMagicResistance,
                _                   => 0f
            };
        }

        private static float CalculateEffectiveResistance(float resistance,
                                                          float flatPenetration,
                                                          float percentPenetration)
        {
            resistance = Mathf.Max(0f, resistance);
            flatPenetration = Mathf.Max(0f, flatPenetration);
            percentPenetration = Mathf.Clamp01(percentPenetration);

            resistance = Mathf.Max(0f, resistance - flatPenetration);
            resistance *= 1f - percentPenetration;
            
            return resistance;
        }

        public static float CalculateDamageForDebug(float damage,
                                                    float resistance,
                                                    float flatPenetration = 0f,
                                                    float percentPenetration = 0f)
        {
            var effectiveResistance = CalculateEffectiveResistance(resistance, flatPenetration, percentPenetration);
            return ApplyResistance(damage, effectiveResistance);
        }

        private static float ApplyResistance(float damage, float resistance)
        {
            damage = Mathf.Max(0f, damage);
            resistance = Mathf.Max(0f, resistance);
            
            return damage * DefenseCoefficient / (DefenseCoefficient + resistance);
        }
    }
}
