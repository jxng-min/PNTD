using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class WarriorSpace : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float lifetime = 1f;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D hitCollider;

        private readonly HashSet<Enemy> _hitEnemies = new();
        private readonly Collider2D[] _overlapResults = new Collider2D[32];

        private Hero _owner;
        private Vector2 _size;
        private float _damageMultiplier;
        private float _timer;
        private bool _isInitialized;
        private bool _disableColliderNextFrame;

        public void Initialize(Vector3 position, Vector2 size, Hero owner, float damageMultiplier)
        {
            _hitEnemies.Clear();

            _owner = owner;
            _size = size;
            _damageMultiplier = damageMultiplier;
            _timer = 0f;
            _disableColliderNextFrame = false;
            _isInitialized = _owner != null;

            transform.position = position;
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            CacheReferences();
            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            if (spriteRenderer != null && _owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _owner.HeroDataTableRow.color;
            }

            if (!_isInitialized)
            {
                ReturnToPool();
                return;
            }

            DamageEnemies();

            if (hitCollider != null)
            {
                _disableColliderNextFrame = true;
            }
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_disableColliderNextFrame)
            {
                _disableColliderNextFrame = false;
                if (hitCollider != null)
                {
                    hitCollider.enabled = false;
                }
            }

            _timer += Time.deltaTime;
            if (_timer >= lifetime)
            {
                ReturnToPool();
            }
        }

        private void DamageEnemies()
        {
            if (hitCollider != null)
            {
                DamageEnemiesWithCollider();
                return;
            }

            DamageEnemiesWithFallbackBox();
        }

        private void DamageEnemiesWithCollider()
        {
            var filter = new ContactFilter2D
            {
                useLayerMask = enemyLayer.value != 0,
                layerMask = enemyLayer,
                useTriggers = true,
            };

            var overlapCount = hitCollider.Overlap(filter, _overlapResults);
            for (var index = 0; index < overlapCount; index++)
            {
                DamageEnemy(_overlapResults[index]);
            }
        }

        private void DamageEnemiesWithFallbackBox()
        {
            var colliders = enemyLayer.value != 0
                ? Physics2D.OverlapBoxAll(transform.position, _size, transform.eulerAngles.z, enemyLayer)
                : Physics2D.OverlapBoxAll(transform.position, _size, transform.eulerAngles.z);

            foreach (var collider2d in colliders)
            {
                DamageEnemy(collider2d);
            }
        }

        private void DamageEnemy(Collider2D collider2d)
        {
            var enemy = collider2d.GetComponentInParent<Enemy>();
            if (!CanHit(enemy))
            {
                return;
            }

            _hitEnemies.Add(enemy);
            var damageContext = _owner.Stat.CreateDamageContext(EAttack.Physical, _damageMultiplier);
            _owner.TryDamagedToEnemy(enemy, damageContext);
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
            _disableColliderNextFrame = false;
            _hitEnemies.Clear();
            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }
        }
    }
}
