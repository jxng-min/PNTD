using JxModule;
using UnityEngine;

namespace PNTD
{
    public class EnemyFactory
    {
        private readonly EnemyBuilder _enemyBuilder;

        private StagePath _stagePath;
        private IEnemyProvider _enemyProvider;

        public EnemyFactory(EnemyBuilder enemyBuilder)
        {
            _enemyBuilder = enemyBuilder;
        }

        public void Initialize(StagePath stagePath, IEnemyProvider enemyProvider)
        {
            _stagePath = stagePath;
            _enemyProvider = enemyProvider;
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

            return Create(enemyContext);
        }

        private Enemy Create(EnemyContext enemyContext)
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
            
            enemy.Initialize(enemyContext, _stagePath, _enemyProvider);
            return enemy;
        }
    }
}