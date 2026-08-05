namespace PNTD
{
    public interface IGlobalEffect
    {
        void Initialize(GlobalEffectContext context);
        void Release();
        void OnStageBegin(REffectContext context);
        void OnEnemyKilled(Hero hero, Enemy enemy);
    }
}