using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class RangerBullet : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float lifetimePadding = 0.1f;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PNTD.ShadowableObject shadow;

        private readonly HashSet<Enemy> _hitEnemies = new();

        private RangerBulletConfig _config;
        private Vector2 _direction;
        private Vector3 _origin;
        private Vector3 _baseVisualScale;
        private float _traveledDistance;
        private int _hitCount;
        private bool _hasBaseVisualScale;
        private bool _isInitialized;

        public void Initialize(Vector3 origin, Vector2 direction, RangerBulletConfig config, bool isHover = false)
        {
            _hitEnemies.Clear();
            
            shadow.SetHoverState(isHover);

            _origin = origin;
            _direction = direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : Vector2.right;
            _config = config;
            _traveledDistance = 0f;
            _hitCount = 0;
            _isInitialized = _config.Owner != null;

            transform.position = _origin;
            ApplyVisualScale();
            SetRotation(_direction);

            CacheReferences();
            if (spriteRenderer != null && _config.Owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _config.Owner.HeroDataTableRow.color;
            }

            if (!_isInitialized)
            {
                ReturnToPool();
            }
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            Move();
            TryHitEnemies();

            if (_traveledDistance >= _config.MaxDistance + lifetimePadding)
            {
                ReturnToPool();
            }
        }

        private void Move()
        {
            var distance = _config.Speed * Time.deltaTime;
            transform.position += (Vector3)(_direction * distance);
            _traveledDistance = Vector3.Distance(_origin, transform.position);
        }

        private void SetRotation(Vector2 direction)
        {
            CacheReferences();

            var target = rotationAxis != null ? rotationAxis : transform;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            target.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void ApplyVisualScale()
        {
            CacheReferences();
            transform.localScale = _baseVisualScale * Mathf.Max(0.01f, _config.VisualScale);
        }

        private void TryHitEnemies()
        {
            var colliders = enemyLayer.value != 0
                ? Physics2D.OverlapCircleAll(transform.position, _config.HitRadius, enemyLayer)
                : Physics2D.OverlapCircleAll(transform.position, _config.HitRadius);

            foreach (var collider2d in colliders)
            {
                var enemy = collider2d.GetComponentInParent<Enemy>();
                if (!CanHit(enemy))
                {
                    continue;
                }

                Hit(enemy);

                if (_hitCount >= _config.MaxPierceCount)
                {
                    ReturnToPool();
                    return;
                }
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

        private void Hit(Enemy enemy)
        {
            _hitEnemies.Add(enemy);
            _hitCount++;

            var damageContext = _config.Owner.Stat.CreateDamageContext(EAttack.Physical, _config.DamageMultiplier);
            _config.Owner.TryDamagedToEnemy(enemy, damageContext, _config.TriggerOnHitEffect);
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

            if (!_hasBaseVisualScale)
            {
                _baseVisualScale = rotationAxis.localScale;
                _hasBaseVisualScale = true;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }
        }

        private void OnDisable()
        {
            _isInitialized = false;
            _hitEnemies.Clear();
        }
    }
}
