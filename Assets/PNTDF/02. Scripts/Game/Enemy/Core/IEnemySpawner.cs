using UnityEngine;

namespace PNTD
{
    public interface IEnemySpawner
    {
        Enemy SpawnEnemy(string enemyId, Vector3 position, int pathPointIndex);
    }
}
