using System;
using System.Collections;
using JxModule;
using JxModule.Timer;
using UnityEngine;

namespace PNTD
{
    public class HeroAttack : MonoBehaviour
    {
        private const float MinimumCooldown = 0.001f;

        private Hero _hero;
        private Coroutine _attackRoutine;
        private bool _isInitialized;

        public event Action<float, float> OnUpdateCool;
        
        public bool IsPaused { get; private set; }
        public bool IsSealed { get; private set; }
        public bool IsAttacking => _attackRoutine != null;

        public float CurrentCoolTime { get; private set; }
        public float MaxCoolTime => _hero.Stat.FinalAttackCooldown;
        public bool NonCool => _hero.Stat.FinalAttackCooldown <= 0.001f;

        public float CooldownRate
        {
            get
            {
                var maxCoolTime = MaxCoolTime;
                if (maxCoolTime <= MinimumCooldown)
                {
                    return 1f;
                }
                
                return Mathf.Clamp01(CurrentCoolTime / maxCoolTime);
            }
        }

        public void Initialize(Hero hero)
        {
            Release();

            if (hero == null)
            {
                DebugExtension.LogColor($"Hero Attack: Hero is null.", Color.red);
                return;
            }
            
            _hero = hero;
            _isInitialized = _hero != null;
            
            IsPaused = false;
            IsSealed = _hero.IsSkillSealed;

            if (_hero.Stat != null)
            {
                _hero.Stat.OnChangedStat += HandleOnChangedStat;
            }

            ResetCool();
        }

        public void ResetCool()
        {
            CurrentCoolTime = 0f;

            if (_hero.Skill.IsContinuous)
            {
                OnUpdateCool?.Invoke(1f, 1f);
                return;
            }
            
            var maxCoolTime = MaxCoolTime;
            OnUpdateCool?.Invoke(CurrentCoolTime, maxCoolTime);
        }

        public void FillCool()
        {
            if (_hero.Skill.IsContinuous)
            {
                OnUpdateCool?.Invoke(1f, 1f);
                return;
            }
            
            CurrentCoolTime = MaxCoolTime;
            OnUpdateCool?.Invoke(CurrentCoolTime, MaxCoolTime);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void SetSealed(bool isSealed)
        {
            IsSealed = isSealed;
        }

        public void CancelAttack()
        {
            if (_attackRoutine == null)
            {
                return;
            }
            
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }

        public void Release()
        {
            CancelAttack();

            if (_hero.Stat != null)
            {
                _hero.Stat.OnChangedStat -= HandleOnChangedStat;
            }
            
            _hero = null;
            CurrentCoolTime = 0f;
            IsPaused = false;
            IsSealed = false;
            
            _isInitialized = false;
        }

        private bool CanProgressAttack()
        {
            if (!_isInitialized || _hero == null || _hero.Skill == null)
            {
                return false;
            }

            if (_hero.Skill.IsContinuous)
            {
                return false;
            }

            if (IsPaused || IsSealed)
            {
                return false;
            }

            if (_attackRoutine != null)
            {
                return false;
            }

            return true;
        }

        private void StartAttack()
        {
            if (_attackRoutine != null)
            {
                return;
            }

            if (_hero == null || _hero.Skill == null || _hero.IsSkillSealed)
            {
                return;
            }

            _attackRoutine = StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            if (_hero == null || _hero.Skill == null || _hero.IsSkillSealed)
            {
                _attackRoutine = null;
                yield break;
            }

            var executeRoutine = _hero.Skill.Execute(_hero);
            if (executeRoutine != null)
            {
                yield return executeRoutine;
            }
            
            CurrentCoolTime = 0f;
            OnUpdateCool?.Invoke(CurrentCoolTime, MaxCoolTime);

            _attackRoutine = null;
        }

        private void HandleOnChangedStat()
        {
            if (_hero.Skill.IsContinuous)
            {
                return;
            }
            
            var maxCoolTime = MaxCoolTime;
            CurrentCoolTime = Mathf.Min(CurrentCoolTime, maxCoolTime);
            OnUpdateCool?.Invoke(CurrentCoolTime, maxCoolTime);
        }

        private void Update()
        {
            if (!CanProgressAttack())
            {
                return;
            }

            var maxCoolTime = MaxCoolTime;

            if (maxCoolTime <= MinimumCooldown)
            {
                StartAttack();
                return;
            }
            
            CurrentCoolTime += Time.deltaTime;
            CurrentCoolTime = Mathf.Min(CurrentCoolTime, maxCoolTime);
            OnUpdateCool?.Invoke(CurrentCoolTime, maxCoolTime);

            if (CurrentCoolTime < maxCoolTime)
            {
                return;
            }
            
            StartAttack();
        }

        private void OnDisable()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            Release();
        }
    }
}