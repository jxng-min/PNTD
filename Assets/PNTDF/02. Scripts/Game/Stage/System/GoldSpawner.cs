using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class GoldSpawner : IGoldSpawner
    {
        private const string GoldPrefabName = "[PF] Gold";

        private readonly RStageContext _runtimeStageContext;
        private readonly Transform _root;
        private readonly List<Gold> _activeGolds = new();

        public GoldSpawner(RStageContext runtimeStageContext, Transform root)
        {
            _runtimeStageContext = runtimeStageContext;
            _root = root;
        }

        public void Spawn(Vector3 center, int goldAmount, float minOffset = 0.15f, float maxOffset = 0.35f)
        {
            var goldPrefab = PrefabManager.CachePrefab<Gold>(GoldPrefabName);
            if (goldPrefab == null)
            {
                DebugExtension.LogColor($"GoldSpawner: Gold prefab '{GoldPrefabName}' is missing.", Color.red);
                return;
            }

            var goldObject = ObjectPoolManager.Instance.Get(goldPrefab.gameObject);
            if (goldObject == null)
            {
                return;
            }

            goldObject.transform.SetParent(_root, false);
            goldObject.transform.position = GetSpawnPosition(center, minOffset, maxOffset);

            var gold = goldObject.GetComponent<Gold>();
            if (gold == null)
            {
                ObjectPoolManager.Instance.Return(goldObject);
                return;
            }

            _activeGolds.Add(gold);
            gold.Initialize(goldAmount, CollectGold);
        }

        public void Clear()
        {
            for (var index = _activeGolds.Count - 1; index >= 0; index--)
            {
                var gold = _activeGolds[index];
                if (gold == null)
                {
                    continue;
                }

                ObjectPoolManager.Instance.Return(gold.gameObject);
            }

            _activeGolds.Clear();
        }

        private void CollectGold(Gold gold, int amount)
        {
            _activeGolds.Remove(gold);
            _runtimeStageContext?.UpdateGold(amount);
        }

        private static Vector3 GetSpawnPosition(Vector3 center, float minOffset, float maxOffset)
        {
            var min = Mathf.Max(0f, Mathf.Min(minOffset, maxOffset));
            var max = Mathf.Max(min, maxOffset);
            var direction = Random.insideUnitCircle;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                direction = Vector2.right;
            }

            direction.Normalize();
            return center + (Vector3)(direction * Random.Range(min, max));
        }
    }
}
