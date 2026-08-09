using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StagePooledObjectCleaner
    {
        private static readonly string[] StagePoolPrefabNames =
        {
            "[PF] Enemy",
            "[PF] Ranger Bullet",
            "[PF] Mage Sphere",
            "[PF] Mage Circle",
            "[PF] Starborn Orb",
            "[PF] Cleric Sancutuary",
            "[PF] Gold",
            "[PF] Warrior Space",
            "[PF] Warrior Fan",
            "[PF] Warrior Axe",
            "[PF] Rogue Crow",
            "[PF] Rogue Dagger",
        };

        public void ReturnStagePooledObjects()
        {
            foreach (var prefabName in StagePoolPrefabNames)
            {
                var prefab = PrefabManager.CachePrefab<Transform>(prefabName);
                if (prefab == null)
                {
                    continue;
                }

                ObjectPoolManager.Instance.ReturnSpecificPoolAll(prefab.gameObject);
            }
        }
    }
}
