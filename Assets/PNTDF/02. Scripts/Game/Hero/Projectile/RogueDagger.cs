using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class RogueDagger : MonoBehaviour
    {
        private const string BleedEffectId = "ThiefBleed";

        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float returnDistance = 0.05f;
        [SerializeField] private float rotationSpeed = 720f;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private readonly HashSet<Enemy> _hitEnemies = new();

        private RogueDaggerConfig _config;
        private Vector2 _direction;
        private Vector3 _origin;
        private float _traveledDistance;
        private float _directionAngle;
        private float _spinAngle;
        private bool _isReturning;
        private bool _isInitialized;

        public void Initialize(Vector3 origin, Vector2 direction, RogueDaggerConfig config)
        {
            _hitEnemies.Clear();

            _origin = origin;
            _direction = direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : Vector2.right;
            _config = config;
            _traveledDistance = 0f;
            _spinAngle = 0f;
            _isReturning = false;
            _isInitialized = _config.Owner != null;

            transform.position = _origin;
            SetRotation(_direction);
            CacheReferences();
            ApplyOwnerColor();

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
                var distance = _config.Speed * Time.deltaTime;
                transform.position += (Vector3)(_direction * distance);
                _traveledDistance = Vector3.Distance(_origin, transform.position);

                if (_traveledDistance >= _config.MaxDistance)
                {
                    BeginReturn();
                }

                return;
            }

            if (_config.Owner == null)
            {
                ReturnToPool();
                return;
            }

            var ownerPosition = GetOwnerPosition();
            var toOwner = ownerPosition - transform.position;
            if (toOwner.magnitude <= returnDistance)
            {
                ReturnToPool();
                return;
            }

            var returnDirection = ((Vector2)toOwner).normalized;
            transform.position += (Vector3)(returnDirection * (_config.Speed * Time.deltaTime));
            SetRotation(returnDirection);
        }

        private void BeginReturn()
        {
            _hitEnemies.Clear();
            _isReturning = true;
        }

        private Vector3 GetOwnerPosition()
        {
            var owner = _config.Owner;
            return owner != null && owner.Model != null && owner.Model.RotationAxis != null
                ? owner.Model.RotationAxis.position
                : owner != null
                    ? owner.transform.position
                    : transform.position;
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

            var owner = _config.Owner;
            if (owner == null || owner.Stat == null)
            {
                return;
            }

            var damageContext = owner.Stat.CreateDamageContext(EAttack.Physical, _config.DamageMultiplier);
            owner.TryDamagedToEnemy(enemy, damageContext);
            ApplyBleed(enemy);
        }

        private void ApplyBleed(Enemy enemy)
        {
            var owner = _config.Owner;
            if (owner == null || enemy?.Status == null)
            {
                return;
            }

            var damagePerTick = owner.Stat.FinalPhysicalAttackPower * _config.BleedDamageMultiplier;
            enemy.Status.AddDoTEffect(BleedEffectId,
                                      damagePerTick,
                                      _config.BleedTickInterval,
                                      _config.BleedDuration,
                                      owner.Stat.FinalFlatPhysicalPenetration,
                                      owner.Stat.FinalPercentPhysicalPenetration,
                                      EAttack.Physical,
                                      owner.HeroDataTableRow.color);
        }

        private void SetRotation(Vector2 direction)
        {
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

        private void ApplyOwnerColor()
        {
            if (spriteRenderer != null && _config.Owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _config.Owner.HeroDataTableRow.color;
            }
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
