using System;
using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class HeroEffector : MonoBehaviour
    {
        private readonly List<HeroEffect> _effects = new();
        private readonly List<HeroEffect> _callbackSnapshot = new();

        private Hero _hero;
        private bool _isInitialized;
        
        public event Action<bool> OnSkillSealChanged;

        public bool IsSkillSealed { get; private set; }
        public IReadOnlyList<HeroEffect> Effects => _effects;

        public void Initialize(Hero hero)
        {
            if (_isInitialized)
            {
                Clear();
            }
            
            _hero = hero;
            _isInitialized = _hero != null;

            RefreshState(true);
        }

        public bool TryAdd(HeroEffect effect)
        {
            if (!_isInitialized || _hero == null || effect == null)
            {
                return false;
            }

            if (_effects.Contains(effect))
            {
                return false;
            }
            
            _effects.Add(effect);
            effect.Attach(_hero);

            if (effect is GlobalEffect globalEffect)
            {
                GlobalEffectRegistry.Register(globalEffect);
            }

            RefreshState();
            return true;
        }

        public bool TryRemove(HeroEffect effect)
        {
            if (effect == null)
            {
                return false;
            }
            
            var index = _effects.IndexOf(effect);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            RefreshState();

            return true;
        }

        public int RemoveEffectsFromType<T>() where T : HeroEffect
        {
            var removedCount = 0;

            for (var index = _effects.Count - 1; index >= 0; index--)
            {
                if (_effects[index] is not T)
                {
                    continue;
                }

                RemoveAt(index);
                removedCount++;
            }

            if (removedCount > 0)
            {
                RefreshState();
            }
            
            return removedCount;
        }

        public void Clear()
        {
            for (var index = _effects.Count - 1; index >= 0; index--)
            {
                RemoveAt(index);
            }
            
            _effects.Clear();
            RefreshState();
        }

        public void NotifyHitEnemy(Enemy enemy)
        {
            if (_hero == null || enemy == null)
            {
                return;
            }

            CreateCallbackSnapshot();

            foreach (HeroEffect effect in _callbackSnapshot)
            {
                if (!IsEffectActive(effect))
                {
                    continue;
                }

                effect.OnHitEnemy(_hero, enemy);
            }

            _callbackSnapshot.Clear();
        }

        public float ModifyDamageToEnemy(Enemy enemy, float damage)
        {
            if (_hero == null || enemy == null || damage <= 0f)
            {
                return damage;
            }

            CreateCallbackSnapshot();

            var modifiedDamage = damage;
            foreach (HeroEffect effect in _callbackSnapshot)
            {
                if (!IsEffectActive(effect))
                {
                    continue;
                }

                modifiedDamage = effect.ModifyDamageToEnemy(_hero, enemy, modifiedDamage);
            }

            _callbackSnapshot.Clear();
            return modifiedDamage;
        }
        
        public void NotifyAffectedEnemy(Enemy enemy)
        {
            if (_hero == null || enemy == null)
            {
                return;
            }

            CreateCallbackSnapshot();

            foreach (HeroEffect effect in _callbackSnapshot)
            {
                if (!IsEffectActive(effect))
                {
                    continue;
                }

                effect.OnAffectEnemy(_hero, enemy);
            }

            _callbackSnapshot.Clear();
        }

        private void RemoveAt(int index)
        {
            if (index < 0 || index >= _effects.Count)
            {
                return;
            }
            
            var effect = _effects[index];
            _effects.RemoveAt(index);

            if (effect == null)
            {
                return;
            }

            if (effect is GlobalEffect globalEffect)
            {
                GlobalEffectRegistry.Unregister(globalEffect);
            }

            _hero?.Stat?.RemoveModifiersFrom(effect);
            effect.Release(_hero);
        }

        private void RefreshState(bool forceNotify = false)
        {
            var isSkillSealed = CalculateSkillSealed();

            if (!forceNotify && isSkillSealed == IsSkillSealed)
            {
                return;
            }
            
            IsSkillSealed = isSkillSealed;
            OnSkillSealChanged?.Invoke(IsSkillSealed);
        }

        private bool CalculateSkillSealed()
        {
            foreach (HeroEffect effect in _effects)
            {
                if (effect is { IsFinished: false, BlockSkill: true })
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEffectActive(HeroEffect effect)
        { 
            return effect != null && !effect.IsFinished && _effects.Contains(effect);
        }

        private void CreateCallbackSnapshot()
        {
            _callbackSnapshot.Clear();

            foreach (HeroEffect effect in _effects)
            {
                if (effect != null)
                {
                    _callbackSnapshot.Add(effect);
                }
            }
        }

        private void Update()
        {
            if (!_isInitialized || _hero == null)
            {
                return;
            }

            var changed = false;

            for (var i = _effects.Count - 1; i >= 0; i--)
            {
                var effect = _effects[i];
                if (effect == null)
                {
                    _effects.RemoveAt(i);
                    changed = true;
                    continue;
                }

                effect.Tick(_hero, Time.deltaTime);

                if (i >= _effects.Count || !ReferenceEquals(_effects[i], effect))
                {
                    changed = true;
                    continue;
                }

                if (!effect.IsFinished)
                {
                    continue;
                }
                
                RemoveAt(i);
                changed = true;
            }

            if (changed)
            {
                RefreshState();
            }
        }

        private void OnDestroy()
        {
            Clear();
            
            OnSkillSealChanged = null;
            _hero = null;
            _isInitialized = false;
        }
    }
}
