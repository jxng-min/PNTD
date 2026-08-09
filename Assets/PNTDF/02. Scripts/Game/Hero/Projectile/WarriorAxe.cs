using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class WarriorAxe : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float returnDistance = 0.15f;
        [SerializeField] private float rotationSpeed = 720f;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private readonly HashSet<Enemy> _hitEnemies = new();

        private Hero _owner;
        private Vector2 _direction;
        private Vector3 _origin;
        private float _damageMultiplier;
        private float _speed;
        private float _maxDistance;
        private float _hitRadius;
        private float _directionAngle;
        private float _spinAngle;
        private bool _canReturn;
        private bool _isReturning;
        private bool _isInitialized;

        public void Initialize(Hero owner,
                               Vector3 origin,
                               Vector2 direction,
                               float damageMultiplier,
                               float speed,
                               float maxDistance,
                               float hitRadius,
                               bool canReturn)
        {
            _hitEnemies.Clear();

            _owner = owner;
            _origin = origin;
            _direction = direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : Vector2.right;
            _damageMultiplier = damageMultiplier;
            _speed = speed;
            _maxDistance = maxDistance;
            _hitRadius = hitRadius;
            _spinAngle = 0f;
            _canReturn = canReturn;
            _isReturning = false;
            _isInitialized = _owner != null;

            transform.position = _origin;
            SetRotation(_direction);

            CacheReferences();
            if (spriteRenderer != null && _owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _owner.HeroDataTableRow.color;
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
            Rotate();
            TryHitEnemies();
        }

        private void Move()
        {
            if (!_isReturning)
            {
                var distance = _speed * Time.deltaTime;
                transform.position += (Vector3)(_direction * distance);

                if (Vector3.Distance(_origin, transform.position) >= _maxDistance)
                {
                    if (_canReturn)
                    {
                        _isReturning = true;
                        _hitEnemies.Clear();
                    }
                    else
                    {
                        ReturnToPool();
                    }
                }

                return;
            }

            if (_owner == null)
            {
                ReturnToPool();
                return;
            }

            var targetPosition = _owner.Model != null && _owner.Model.RotationAxis != null
                ? _owner.Model.RotationAxis.position
                : _owner.transform.position;
            var toOwner = targetPosition - transform.position;
            if (toOwner.magnitude <= returnDistance)
            {
                ReturnToPool();
                return;
            }

            var returnDirection = ((Vector2)toOwner).normalized;
            transform.position += (Vector3)(returnDirection * (_speed * Time.deltaTime));
            SetRotation(returnDirection);
        }

        private void SetRotation(Vector2 direction)
        {
            CacheReferences();

            _directionAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            ApplyRotation();
        }

        private void Rotate()
        {
            _spinAngle += rotationSpeed * Time.deltaTime;
            ApplyRotation();
        }

        private void ApplyRotation()
        {
            var target = rotationAxis != null ? rotationAxis : transform;
            target.rotation = Quaternion.Euler(0f, 0f, _directionAngle + _spinAngle);
        }

        private void TryHitEnemies()
        {
            var colliders = enemyLayer.value != 0
                ? Physics2D.OverlapCircleAll(transform.position, _hitRadius, enemyLayer)
                : Physics2D.OverlapCircleAll(transform.position, _hitRadius);

            foreach (var collider2d in colliders)
            {
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
        }

        private void OnDisable()
        {
            _isInitialized = false;
            _hitEnemies.Clear();
        }
    }
}
