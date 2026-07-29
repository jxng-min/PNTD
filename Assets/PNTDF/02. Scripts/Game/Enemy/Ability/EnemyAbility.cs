namespace PNTD
{
    public abstract class EnemyAbility
    {
        public abstract void Initialize(EnemyAbilityContext abilityContext);
        
        public virtual void Tick(float deltaTime, bool isCooldownPaused) {}
        public virtual void FixedTick(float deltaTime) {}
        public virtual void LateTick(float deltaTime) {}

        public abstract void Release();
    }
}