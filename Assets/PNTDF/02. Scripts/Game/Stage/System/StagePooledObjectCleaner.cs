using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StagePooledObjectCleaner
    {
        private static readonly string[] StagePoolPrefabNames =
        {
            "[PF] Ranger Bullet",
            "[PF] Mage Sphere",
            "[PF] Mage Circle",
            "[PF] Starborn Orb",
            "[PF] Cleric Sancutuary",
            "[PF] Gold",
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
