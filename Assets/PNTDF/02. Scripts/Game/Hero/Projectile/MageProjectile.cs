using JxModule;
using UnityEngine;

namespace PNTD
{
    public class MageProjectile : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float lifetimePadding = 0.1f;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private MageProjectileConfig _config;
        private Vector2 _direction;
        private Vector3 _origin;
        private float _traveledDistance;
        private bool _isInitialized;

        public void Initialize(Vector3 origin, Vector2 direction, MageProjectileConfig config)
        {
            _origin = origin;
            _direction = direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : Vector2.right;
            _config = config;
            _traveledDistance = 0f;
            _isInitialized = _config.Owner != null;

            transform.position = _origin;
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
            if (TryHitEnemy())
            {
                ReturnToPool();
                return;
            }

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

        private bool TryHitEnemy()
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
                return true;
            }

            return false;
        }

        private static bool CanHit(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null &&
                   !enemy.Health.IsDead;
        }

        private void Hit(Enemy enemy)
        {
            var owner = _config.Owner;
            if (owner == null || owner.Stat == null)
            {
                return;
            }

            var damageContext = owner.Stat.CreateDamageContext(EAttack.Magic, _config.DamageMultiplier);
            owner.TryDamagedToEnemy(enemy, damageContext, _config.TriggerOnHitEffect);
            if (enemy.Health.IsDead)
            {
                return;
            }
            
            ApplyDot(enemy);
            ApplyControlEffects(enemy);
        }

        private void ApplyDot(Enemy enemy)
        {
            var owner = _config.Owner;
            if (owner == null || owner.Stat == null || enemy == null || enemy.Status == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_config.DotEffectId) ||
                _config.DotDamageMultiplier <= 0f ||
                _config.DotTickInterval <= 0f ||
                _config.DotDuration <= 0f)
            {
                return;
            }

            var damagePerTick = owner.Stat.FinalMagicAttackPower * _config.DotDamageMultiplier;
            enemy.Status.AddDoTEffect(_config.DotEffectId,
                                      damagePerTick,
                                      _config.DotTickInterval,
                                      _config.DotDuration,
                                      owner.Stat.FinalFlatMagicPenetration,
                                      owner.Stat.FinalPercentMagicPenetration,
                                      EAttack.Magic);
        }
        
        private void ApplyControlEffects(Enemy enemy)
        {
            if (enemy == null || enemy.Status == null)
            {
                return;
            }

            TryApplyDisable(enemy);
            TryApplyStun(enemy);
        }

        private void TryApplyDisable(Enemy enemy)
        {
            if (string.IsNullOrWhiteSpace(_config.DisableEffectId) ||
                _config.DisableChance <= 0f ||
                _config.DisableDuration <= 0f)
            {
                return;
            }

            if (Random.value > Mathf.Clamp01(_config.DisableChance))
            {
                return;
            }

            enemy.Status.AddDisabledEffect(_config.DisableEffectId,
                                           _config.DisableDuration,
                                           _config.DisableOverrideColor);
        }

        private void TryApplyStun(Enemy enemy)
        {
            if (string.IsNullOrWhiteSpace(_config.StunEffectId) ||
                _config.StunDuration <= 0f)
            {
                return;
            }

            enemy.Status.AddStunEffect(_config.StunEffectId,
                                       _config.StunDuration,
                                       _config.StunOverrideColor);
        }

        private void ReturnToPool()
        {
            _isInitialized = false;
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
        }
    }
}
