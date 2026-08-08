namespace PNTD
{
    public class EnemyAbilityContext
    {
        public Enemy Owner { get; }
        public IEnemyProvider EnemyProvider { get; }
        public IEnemySpawner EnemySpawner { get; }
        public IHeroProvider HeroProvider { get; }

        public EnemyAbilityContext(Enemy owner,
                                   IEnemyProvider enemyProvider,
                                   IEnemySpawner enemySpawner,
                                   IHeroProvider heroProvider)
        {
            Owner = owner;
            EnemyProvider = enemyProvider;
            EnemySpawner = enemySpawner;
            HeroProvider = heroProvider;
        }
    }
}
