using UnityEngine;

namespace PNTD
{
    public abstract class EnemyStatusEffect
    {
        private bool _isInitialized;
        
        public float Duration { get; }
        public float RemainingDuration { get; private set; }

        public bool IsFinished { get; private set; }
        protected Enemy Owner { get; private set; }
        public abstract string EffectID { get; }
        
        public Color? OverrideColor { get; }
        public virtual float MoveSpeedMultiplier => 1f;
        public virtual float PhysicalDefenseModifier => 0f;
        public virtual float MagicResistanceModifier => 0f;
        public virtual float SlowResistanceModifier => 0f;
        public virtual EStackPolicy StackPolicy => EStackPolicy.Stack;
        public virtual EEffectCategory Category => EEffectCategory.Debuff;
        public virtual float Strength => 0f;

        public virtual bool IsStun => false;
        public virtual bool IsDisabled => false;
        public virtual bool IsInvincible => false;
        public virtual bool IsEffectImmune => false;

        protected EnemyStatusEffect(float duration, Color? overrideColor = null)
        {
            Duration = Mathf.Max(0f, duration);
            OverrideColor = overrideColor;
        }

        public void Initialize(Enemy owner)
        {
            if (owner == null || _isInitialized)
            {
                return;
            }
            
            Owner = owner;
            RemainingDuration = Duration;
            IsFinished = false;
            _isInitialized = true;

            OnApply();
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized || IsFinished)
            {
                return;
            }

            var safeDeltaTime = Mathf.Max(0f, deltaTime);
            RemainingDuration -= safeDeltaTime;
            Tick(safeDeltaTime);

            if (RemainingDuration <= 0f)
            {
                Finish();
            }
        }

        public void RefreshDuration(float duration)
        {
            RemainingDuration = Mathf.Max(0f, duration);
        }

        public void ExtendDuration(float duration)
        {
            RemainingDuration += Mathf.Max(0f, duration);
        }

        public void RemoveImmediately()
        {
            if (!_isInitialized || IsFinished)
            {
                return;
            }

            Finish();
        }

        private void Finish()
        {
            IsFinished = true;
            RemainingDuration = 0f;
            
            OnRemove();
        }
        
        protected virtual void OnApply() {}
        protected virtual void Tick(float deltaTime) {}
        protected virtual void OnRemove() {}
    }
}