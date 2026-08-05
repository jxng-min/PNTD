using UnityEngine;

namespace PNTD
{
    public abstract class HeroEffect
    {
        public bool IsFinished { get; protected set; }
        public virtual bool BlockSkill => false;
        public virtual Color? OverrideColor => null;

        public void Attach(Hero hero)
        {
            if (hero == null || IsFinished)
            {
                return;
            }

            OnApply(hero);
        }

        public virtual void Tick(Hero hero, float deltaTime) {}
        public virtual void Release(Hero hero) {}
        public virtual void OnHitEnemy(Hero hero, Enemy enemy) {}
        public virtual void OnAffectEnemy(Hero hero, Enemy enemy) {}
        
        protected abstract void OnApply(Hero hero);
    }
}
