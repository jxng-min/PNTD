using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ClericSanctuaryController : MonoBehaviour
    {
        private const string SanctuaryPrefabName = "[PF] Cleric Sancutuary";
        private const int EnemyOverlapCapacity = 64;

        private readonly Collider2D[] _enemyOverlapBuffer = new Collider2D[EnemyOverlapCapacity];
        private readonly HashSet<Enemy> _trackedEnemies = new();
        private readonly HashSet<Enemy> _currentEnemies = new();
        private readonly Dictionary<Enemy, float> _nextDamageTimes = new();

        private Hero _owner;
        private ClericSanctuaryData _data;
        private float _radiusMultiplier = 1f;
        private ClericSanctuaryVisual _visual;
        private float _visualRadius = -1f;
        private bool _isInitialized;

        public Hero Owner => _owner;
        public ClericSanctuaryData Data => _data;
        public bool IsAllyAura => _data != null && _data.auraType != EClericAuraType.None;
        public float Radius => (_data?.radius ?? 0f) * _radiusMultiplier;
        public float CurrentValue => _data != null && _owner != null ? _data.GetLevelValue(_owner.Level) : 0f;
        public float JudgedDamageTakenBonus => _data != null && _owner != null ? _data.GetJudgedDamageTakenBonus(_owner.Level) : 0f;

        public void Initialize(Hero owner, ClericSanctuaryData data)
        {
            _owner = owner;
            _data = data;
            _radiusMultiplier = 1f;
            _nextDamageTimes.Clear();
            _isInitialized = _owner != null && _data != null;

            EnsureVisual();
            UpdateVisual(true);
        }

        public void SetRadiusMultiplier(float radiusMultiplier)
        {
            _radiusMultiplier = Mathf.Max(0f, radiusMultiplier);
            UpdateVisual(false);
        }

        public bool Contains(Hero hero)
        {
            if (!_isInitialized || hero == null)
            {
                return false;
            }

            var radius = Radius;
            var distanceSqr = (hero.transform.position - transform.position).sqrMagnitude;
            return distanceSqr <= radius * radius;
        }

        public void Release()
        {
            ClearJudgedEnemies();
            _owner = null;
            _data = null;
            _trackedEnemies.Clear();
            _currentEnemies.Clear();
            _nextDamageTimes.Clear();
            _radiusMultiplier = 1f;
            _isInitialized = false;
            ReturnVisualToPool();
        }

        private void Update()
        {
            if (!_isInitialized || IsAllyAura || _owner == null || _owner.IsSkillSealed)
            {
                UpdateVisual(false);
                return;
            }

            UpdateVisual(false);
            RefreshJudgedEnemies();
        }

        private void RefreshJudgedEnemies()
        {
            var radius = Radius;
            var count = Physics2D.OverlapCircle(transform.position, radius, ContactFilter2D.noFilter, _enemyOverlapBuffer);
            var nextInterval = Mathf.Max(0.001f, _data.tickInterval);
            _currentEnemies.Clear();

            for (var index = 0; index < count; index++)
            {
                var enemy = _enemyOverlapBuffer[index] != null ? _enemyOverlapBuffer[index].GetComponentInParent<Enemy>() : null;
                if (!IsValid(enemy))
                {
                    continue;
                }

                _currentEnemies.Add(enemy);
                enemy.Status?.AddCrusaderJudged(this, JudgedDamageTakenBonus);

                if (!_nextDamageTimes.TryGetValue(enemy, out var nextDamageTime))
                {
                    nextDamageTime = 0f;
                }

                if (Time.time < nextDamageTime)
                {
                    continue;
                }

                var damageContext = _owner.Stat.CreateDamageContext(EAttack.Magic, CurrentValue);
                _owner.TryDamagedToEnemy(enemy, damageContext, false);
                _nextDamageTimes[enemy] = Time.time + nextInterval;
            }

            foreach (var enemy in _trackedEnemies)
            {
                if (enemy != null && !_currentEnemies.Contains(enemy))
                {
                    enemy.Status?.RemoveCrusaderJudged(this);
                    _nextDamageTimes.Remove(enemy);
                }
            }

            _trackedEnemies.Clear();
            foreach (var enemy in _currentEnemies)
            {
                _trackedEnemies.Add(enemy);
            }
        }

        private static bool IsValid(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null &&
                   !enemy.Health.IsDead;
        }

        private void ClearJudgedEnemies()
        {
            if (_data == null || IsAllyAura)
            {
                return;
            }

            foreach (var enemy in _trackedEnemies)
            {
                enemy?.Status?.RemoveCrusaderJudged(this);
            }

            _trackedEnemies.Clear();
            _currentEnemies.Clear();
            _nextDamageTimes.Clear();
        }

        private void EnsureVisual()
        {
            if (_visual != null)
            {
                return;
            }

            var visualPrefab = PrefabManager.CachePrefab<ClericSanctuaryVisual>(SanctuaryPrefabName);
            GameObject visualObject;

            if (visualPrefab == null)
            {
                var transformPrefab = PrefabManager.CachePrefab<Transform>(SanctuaryPrefabName);
                if (transformPrefab == null)
                {
                    Debug.LogWarning($"ClericSanctuaryController: Prefab '{SanctuaryPrefabName}' is missing.");
                    return;
                }

                visualObject = ObjectPoolManager.Instance.Get(transformPrefab.gameObject);
                _visual = visualObject.GetComponent<ClericSanctuaryVisual>();
                if (_visual == null)
                {
                    _visual = visualObject.AddComponent<ClericSanctuaryVisual>();
                }
            }
            else
            {
                visualObject = ObjectPoolManager.Instance.Get(visualPrefab.gameObject);
                _visual = visualObject.GetComponent<ClericSanctuaryVisual>();
            }

            visualObject.name = "[Runtime] Cleric Sanctuary Visual";
            visualObject.transform.SetParent(transform, false);
            visualObject.transform.localPosition = Vector3.zero;
            visualObject.transform.localRotation = Quaternion.identity;
            _visual.Initialize();
        }

        private void UpdateVisual(bool force)
        {
            if (_visual == null)
            {
                return;
            }

            var isVisible = _isInitialized && _data != null && Radius > 0f;
            _visual.gameObject.SetActive(isVisible);
            if (!isVisible)
            {
                _visualRadius = -1f;
                return;
            }

            var color = GetSanctuaryColor();
            _visual.SetColor(color);

            var radius = Radius;
            if (!force && Mathf.Approximately(_visualRadius, radius))
            {
                return;
            }

            _visualRadius = radius;
            _visual.SetRadius(radius);
        }

        private Color GetSanctuaryColor()
        {
            if (_owner?.HeroDataTableRow != null)
            {
                return _owner.HeroDataTableRow.color;
            }

            return Color.white;
        }

        private void ReturnVisualToPool()
        {
            if (_visual == null)
            {
                return;
            }

            var visualObject = _visual.gameObject;
            visualObject.SetActive(false);
            visualObject.transform.SetParent(null);
            ObjectPoolManager.Instance.Return(visualObject);

            _visual = null;
            _visualRadius = -1f;
        }

        private void OnDisable()
        {
            if (!_isInitialized)
            {
                ReturnVisualToPool();
            }
        }

        private void OnDestroy()
        {
            ReturnVisualToPool();
        }
    }
}
