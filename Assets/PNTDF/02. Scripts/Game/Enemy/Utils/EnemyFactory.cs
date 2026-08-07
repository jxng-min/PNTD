using JxModule;
using UnityEngine;

namespace PNTD
{
    public class EnemyFactory
    {
        private readonly EnemyBuilder _enemyBuilder;

        private StagePath _stagePath;
        private IEnemyProvider _enemyProvider;
        private IEnemySpawner _enemySpawner;
        private IHeroProvider _heroProvider;

        public EnemyFactory(EnemyBuilder enemyBuilder)
        {
            _enemyBuilder = enemyBuilder;
        }

        public void Initialize(StagePath stagePath,
                               IEnemyProvider enemyProvider,
                               IEnemySpawner enemySpawner,
                               IHeroProvider heroProvider)
        {
            _stagePath = stagePath;
            _enemyProvider = enemyProvider;
            _enemySpawner = enemySpawner;
            _heroProvider = heroProvider;
        }

        public Enemy Create(string enemyId)
        {
            if (_stagePath == null || string.IsNullOrEmpty(enemyId))
            {
                return null;
            }

            var enemyContext = _enemyBuilder.Build(enemyId);
            if (enemyContext == null)
            {
                DebugExtension.LogColor($"Enemy Factory: Enemy Context not found. Enemy ID: {enemyId}", Color.red);
                return null;
            }

            return Create(enemyContext, null, 1);
        }

        public Enemy Create(string enemyId, Vector3 position, int pathPointIndex)
        {
            if (_stagePath == null || string.IsNullOrEmpty(enemyId))
            {
                return null;
            }

            var enemyContext = _enemyBuilder.Build(enemyId);
            if (enemyContext == null)
            {
                DebugExtension.LogColor($"Enemy Factory: Enemy Context not found. Enemy ID: {enemyId}", Color.red);
                return null;
            }

            return Create(enemyContext, position, pathPointIndex);
        }

        private Enemy Create(EnemyContext enemyContext, Vector3? startPosition, int pathPointIndex)
        {
            var enemyPrefab = PrefabManager.CachePrefab<Enemy>();
            if (enemyPrefab == null)
            {
                return null;
            }

            var enemyObject = ObjectPoolManager.Instance.Get(enemyPrefab.gameObject);
            if (enemyObject == null)
            {
                return null;
            }

            var enemy = enemyObject.GetComponent<Enemy>();
            if (enemy == null)
            {
                return null;
            }
            
            enemy.Initialize(enemyContext,
                             _stagePath,
                             _enemyProvider,
                             _enemySpawner,
                             _heroProvider,
                             startPosition,
                             pathPointIndex);
            return enemy;
        }
    }
}
