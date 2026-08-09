using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class WarriorFan : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D hitCollider;

        private readonly HashSet<Enemy> _hitEnemies = new();
        private readonly Collider2D[] _overlapResults = new Collider2D[32];

        private Hero _owner;
        private float _damageMultiplier;
        private float _orbitRadius;
        private float _degreesPerSecond;
        private int _targetRevolutions;
        private float _angle;
        private int _completedRevolutions;
        private bool _isInitialized;

        public void Initialize(Hero owner,
                               float damageMultiplier,
                               float orbitRadius,
                               float degreesPerSecond,
                               int revolutionCount)
        {
            _hitEnemies.Clear();

            _owner = owner;
            _damageMultiplier = damageMultiplier;
            _orbitRadius = orbitRadius;
            _degreesPerSecond = degreesPerSecond;
            _targetRevolutions = Mathf.Max(1, revolutionCount);
            _angle = 0f;
            _completedRevolutions = 0;

            CacheReferences();
            _isInitialized = _owner != null && hitCollider != null;

            if (spriteRenderer != null && _owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _owner.HeroDataTableRow.color;
            }

            if (!_isInitialized)
            {
                ReturnToPool();
                return;
            }

            UpdatePosition();
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            _angle += _degreesPerSecond * Time.deltaTime;
            while (_angle >= 360f)
            {
                _angle -= 360f;
                _completedRevolutions++;
                _hitEnemies.Clear();

                if (_completedRevolutions >= _targetRevolutions)
                {
                    ReturnToPool();
                    return;
                }
            }

            UpdatePosition();
            TryHitEnemies();
        }

        private void UpdatePosition()
        {
            if (_owner == null)
            {
                ReturnToPool();
                return;
            }

            var radians = _angle * Mathf.Deg2Rad;
            var offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * _orbitRadius;
            transform.position = _owner.transform.position + offset;

            var target = rotationAxis != null ? rotationAxis : transform;
            target.rotation = Quaternion.Euler(0f, 0f, _angle - 90f);
        }

        private void TryHitEnemies()
        {
            if (hitCollider == null)
            {
                ReturnToPool();
                return;
            }

            var filter = new ContactFilter2D
            {
                useLayerMask = enemyLayer.value != 0,
                layerMask = enemyLayer,
                useTriggers = true,
            };

            var overlapCount = hitCollider.Overlap(filter, _overlapResults);
            for (var index = 0; index < overlapCount; index++)
            {
                var collider2d = _overlapResults[index];
                var enemy = collider2d.GetComponentInParent<Enemy>();
                if (!CanHit(enemy))
                {
                    continue;
                }

                _hitEnemies.Add(enemy);
                var damageContext = _owner.Stat.CreateDamageContext(EAttack.Physical, _damageMultiplier);
                _owner.TryDamagedToEnemy(enemy, damageContext);
            }
        }

        private bool CanHit(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null &&
                   !enemy.Health.IsDead &&
                   !_hitEnemies.Contains(enemy);
        }

        private void ReturnToPool()
        {
            _isInitialized = false;
            _hitEnemies.Clear();
            ObjectPoolManager.Instance.Return(gameObject);
        }

        private void CacheReferences()
        {
            if (rotationAxis == null)
            {
                rotationAxis = transform;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (hitCollider == null)
            {
                hitCollider = GetComponentInChildren<Collider2D>(true);
            }
        }

        private void OnDisable()
        {
            _isInitialized = false;
            _hitEnemies.Clear();
        }
    }
}
