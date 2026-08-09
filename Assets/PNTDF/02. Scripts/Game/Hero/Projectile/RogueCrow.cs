using JxModule;
using UnityEngine;

namespace PNTD
{
    public class RogueCrow : MonoBehaviour
    {
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Hero _owner;
        private Enemy _target;
        private float _damageMultiplier;
        private float _speed;
        private float _hitDistance;
        private bool _isInitialized;

        public void Initialize(Vector3 origin,
                               Hero owner,
                               Enemy target,
                               float damageMultiplier,
                               float speed,
                               float hitDistance)
        {
            _owner = owner;
            _target = target;
            _damageMultiplier = damageMultiplier;
            _speed = speed;
            _hitDistance = hitDistance;
            _isInitialized = _owner != null && _target != null;

            transform.position = origin;
            CacheReferences();
            ApplyOwnerColor();
            if (IsValidTarget(_target))
            {
                SetRotation(_target.transform.position - transform.position);
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

            if (!IsValidTarget(_target))
            {
                ReturnToPool();
                return;
            }

            var targetPosition = _target.transform.position;
            var toTarget = targetPosition - transform.position;
            if (toTarget.magnitude <= _hitDistance)
            {
                HitTarget();
                ReturnToPool();
                return;
            }

            var direction = ((Vector2)toTarget).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            SetRotation(direction);
        }

        private void HitTarget()
        {
            if (_owner == null || !IsValidTarget(_target))
            {
                return;
            }

            var damageContext = _owner.Stat.CreateDamageContext(EAttack.Physical, _damageMultiplier);
            _owner.TryDamagedToEnemy(_target, damageContext);
        }

        private void SetRotation(Vector2 direction)
        {
            var target = rotationAxis != null ? rotationAxis : transform;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            target.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void ApplyOwnerColor()
        {
            if (spriteRenderer != null && _owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _owner.HeroDataTableRow.color;
            }
        }

        private static bool IsValidTarget(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null &&
                   !enemy.Health.IsDead;
        }

        private void ReturnToPool()
        {
            _isInitialized = false;
            _owner = null;
            _target = null;
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
            _owner = null;
            _target = null;
        }
    }
}
