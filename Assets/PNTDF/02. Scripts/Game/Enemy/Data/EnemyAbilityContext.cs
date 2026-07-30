namespace PNTD
{
    public class EnemyAbilityContext
    {
        public Enemy Owner { get; }
        public IEnemyProvider EnemyProvider { get; }

        public EnemyAbilityContext(Enemy owner, IEnemyProvider enemyProvider)
        {
            Owner = owner;
            EnemyProvider = enemyProvider;
        }
    }
}