namespace PNTD
{
    public abstract class GlobalEffect : HeroEffect, IGlobalEffect
    {
        public virtual void Initialize(GlobalEffectContext context) {}
        public virtual void Release() {}
        public virtual void OnStageBegin(REffectContext context) {}
        public virtual void OnEnemyKilled(Hero hero, Enemy enemy) {}
        
        protected override void OnApply(Hero hero) {}
    }
}