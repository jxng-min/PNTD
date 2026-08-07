using System;
using UnityEngine;

namespace PNTD
{
    public class PlunderSystem
    {
        private readonly IGoldSpawner _goldSpawner;
        private readonly Func<SynergyContext> _synergyContextProvider;

        public PlunderSystem(IGoldSpawner goldSpawner, Func<SynergyContext> synergyContextProvider)
        {
            _goldSpawner = goldSpawner;
            _synergyContextProvider = synergyContextProvider;
        }

        public void HandleEnemyKilled(Enemy enemy)
        {
            if (enemy == null)
            {
                return;
            }

            var plunderCount = _synergyContextProvider?.Invoke()?.GetCount(ESynergy.Plunder) ?? 0;
            var dropChance = GetDropChance(plunderCount);
            if (dropChance <= 0f || UnityEngine.Random.value > dropChance)
            {
                return;
            }

            _goldSpawner?.Spawn(enemy.transform.position, 1, 0.25f, 0.4f);
        }

        private static float GetDropChance(int plunderCount)
        {
            if (plunderCount >= 4)
            {
                return 0.4f;
            }

            return plunderCount >= 2 ? 0.2f : 0f;
        }
    }
}
