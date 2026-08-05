using System;
using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class EnemyStatus : MonoBehaviour
    {
        private readonly List<EnemyStatusEffect> _effects = new();

        private Enemy _owner;

        private float _baseMoveSpeed;
        private float _basePhysicalDefense;
        private float _baseMagicResistance;
        private float _baseSlowResistance;

        private bool _isTrapped;

        public event Action OnChanged;

        public bool IsStunned => HasEffect(effect => effect.IsStun);
        public bool IsDisabled => HasEffect(effect => effect.IsDisabled);
        public bool IsInvincible => HasEffect(effect => effect.IsInvincible);
        public bool IsEffectImmune => HasEffect(effect => effect.IsEffectImmune);
        
        public bool IsMovementDisabled => IsStunned || _isTrapped;
        public bool IsAbilityDisabled => IsStunned || IsDisabled;

        public float FinalPhysicalDefense
        {
            get
            {
                var defense = _basePhysicalDefense;

                foreach (var effect in _effects)
                {
                    if (effect.IsFinished)
                    {
                        continue;
                    }

                    defense += effect.PhysicalDefenseModifier;
                }

                return Mathf.Max(0f, defense);
            }
        }

        public float FinalMagicResistance
        {
            get
            {
                var resistance = _baseMagicResistance;

                foreach (var effect in _effects)
                {
                    if (effect.IsFinished)
                    {
                        continue;
                    }

                    resistance += effect.MagicResistanceModifier;
                }
                
                return Mathf.Max(0f, resistance);
            }
        }

        public float FinalSlowResistance
        {
            get
            {
                var resistance = _baseSlowResistance;

                foreach (var effect in _effects)
                {
                    if (effect.IsFinished)
                    {
                        continue;
                    }

                    resistance += effect.SlowResistanceModifier;
                }
                
                return Mathf.Max(0f, resistance);
            }
        }

        public float FinalMoveSpeed
        {
            get
            {
                if (IsMovementDisabled)
                {
                    return 0f;
                }
                
                var speed = _baseMoveSpeed;

                foreach (var effect in _effects)
                {
                    if (effect.IsFinished)
                    {
                        continue;
                    }

                    var multiplier = effect.MoveSpeedMultiplier;
                    if (multiplier >= 1f)
                    {
                        speed *= multiplier;
                        continue;
                    }

                    var slowRate = 1f - multiplier;
                    var resistedSlowRate = slowRate * (1f - FinalSlowResistance);
                    
                    speed *= 1f - resistedSlowRate;
                }

                var minimumSpeed = _baseMoveSpeed * 0.2f;
                return Mathf.Max(speed, minimumSpeed);
            }
        }

        public Color? FinalOverrideColor
        {
            get
            {
                for (var i = _effects.Count - 1; i >= 0; i--)
                {
                    var effect = _effects[i];

                    if (effect.IsFinished || !effect.OverrideColor.HasValue)
                    {
                        continue;
                    }

                    return effect.OverrideColor.Value;
                }

                return null;
            }
        }

        public void Initialize(Enemy owner,
                               float baseMoveSpeed,
                               float basePhysicalDefense,
                               float baseMagicResistance,
                               float baseSlowResistance)
        {
            ClearEffects();
            
            _owner = owner;
            _baseMoveSpeed = baseMoveSpeed;
            _basePhysicalDefense = basePhysicalDefense;
            _baseMagicResistance = baseMagicResistance;
            _baseSlowResistance = baseSlowResistance;
            _isTrapped = false;
            
            OnChanged?.Invoke();
        }

        public bool AddMoveSpeedEffect(string effectId, 
                                       float multiplier, 
                                       float duration, 
                                       Color? overrideColor = null,
                                       EStackPolicy stackPolicy = EStackPolicy.KeepStrongest)
        {
            return AddEffect(new EnemyMoveSpeedEffect(effectId, multiplier, duration, stackPolicy, overrideColor));
        }

        public bool AddPhysicalDefenseEffect(string effectId, float modifier, float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemyPhysicalDefenseEffect(effectId, modifier, duration, overrideColor));
        }

        public bool AddMagicResistanceEffect(string effectId, float modifier, float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemyMagicResistanceEffect(effectId, modifier, duration, overrideColor));
        }

        public bool AddSlowResistanceEffect(string effectId, float modifier, float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemySlowResistanceEffect(effectId, modifier, duration, overrideColor));
        }

        public bool AddStunEffect(string effectId, float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemyStunEffect(effectId, duration, overrideColor));
        }

        public bool AddDisabledEffect(string effectId, float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemyDisableEffect(effectId, duration, overrideColor));
        }

        public bool AddDoTEffect(string effectId, float damagePerTick, float tickInterval, float duration, float flatPenetration, float percentPenetration, EAttack attackType, Color? overrideColor = null)
        {
            return AddEffect(new EnemyDoTEffect(effectId, damagePerTick, tickInterval, duration, flatPenetration, percentPenetration, attackType, overrideColor));
        }

        public bool AddInvincibleEffect(float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemyInvincibleEffect(duration, overrideColor));
        }

        public bool AddImmuneEffect(float duration, Color? overrideColor = null)
        {
            return AddEffect(new EnemyImmuneEffect(duration, overrideColor));
        }

        public void EnableMovement()
        {
            if (!_isTrapped)
            {
                return;
            }
            
            _isTrapped = false;
            OnChanged?.Invoke();
        }

        public void DisableMovement()
        {
            if (_isTrapped)
            {
                return;
            }
            
            _isTrapped = true;
            OnChanged?.Invoke();
        }
        
        public bool AddEffect(EnemyStatusEffect newEffect)
        {
            if (newEffect == null || _owner == null)
            {
                return false;
            }

            if (IsEffectImmune && newEffect.Category != EEffectCategory.Buff)
            {
                return false;
            }

            var existingEffect = FindEffect(newEffect.EffectID);
            if (existingEffect == null)
            {
                RegisterEffect(newEffect);
                return false;
            }

            switch (newEffect.StackPolicy)
            {
                case EStackPolicy.Stack:
                    RegisterEffect(newEffect);
                    break;
                
                case EStackPolicy.RefreshDuration:
                    existingEffect.RefreshDuration(newEffect.Duration);
                    OnChanged?.Invoke();
                    break;
                
                case EStackPolicy.ExtendDuration:
                    existingEffect.ExtendDuration(newEffect.Duration);
                    OnChanged?.Invoke();
                    break;
                
                case EStackPolicy.Replace:
                    ReplaceEffect(existingEffect, newEffect);
                    break;
                
                case EStackPolicy.KeepStrongest:
                    ApplyStrongestEffect(existingEffect, newEffect);
                    break;
                
                default:
                    return false;
            }

            return true;
        }
        
        public void ClearEffects()
        {
            foreach (var effect in _effects)
            {
                effect.RemoveImmediately();
            }
            
            _effects.Clear();
        }

        private EnemyStatusEffect FindEffect(string effectId)
        {
            foreach (var effect in _effects)
            {
                if (effect.IsFinished)
                {
                    continue;
                }

                if (effect.EffectID == effectId)
                {
                    return effect;
                }
            }

            return null;
        }

        private void RegisterEffect(EnemyStatusEffect effect)
        {
            effect.Initialize(_owner);
            _effects.Add(effect);
            OnChanged?.Invoke();
        }

        private void ReplaceEffect(EnemyStatusEffect existingEffect, EnemyStatusEffect newEffect)
        {
            existingEffect.RemoveImmediately();
            _effects.Remove(existingEffect);
            RegisterEffect(newEffect);
        }

        private void ApplyStrongestEffect(EnemyStatusEffect existingEffect, EnemyStatusEffect newEffect)
        {
            if (newEffect.Strength > existingEffect.Strength)
            {
                ReplaceEffect(existingEffect, newEffect);
                return;
            }

            if (newEffect.Duration > existingEffect.RemainingDuration)
            {
                existingEffect.RefreshDuration(newEffect.Duration);
                OnChanged?.Invoke();
            }
        }

        private bool HasEffect(Predicate<EnemyStatusEffect> predicate)
        {
            foreach (var effect in _effects)
            {
                if (effect.IsFinished)
                {
                    continue;
                }

                if (predicate(effect))
                {
                    return true;
                }
            }

            return false;
        }

        private void Update()
        {
            var isChanged = false;

            for (var i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];
                
                effect.Update(Time.deltaTime);
                if (!effect.IsFinished)
                {
                    continue;
                }
                
                _effects.RemoveAt(i);
                isChanged = true;
            }

            if (isChanged)
            {
                OnChanged?.Invoke();
            }
        }
    }
}
