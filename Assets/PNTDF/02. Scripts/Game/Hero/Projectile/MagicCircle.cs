using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class MagicCircle : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;

        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CircleCollider2D circleCollider;

        private readonly HashSet<Enemy> _affectedEnemies = new();
        private readonly Collider2D[] _overlapResults = new Collider2D[32];

        private MagicCircleConfig _config;
        private Vector3 _baseScale;
        private float _elapsedTime;
        private float _tickElapsedTime;
        private bool _hasBaseScale;
        private bool _isActivated;
        private bool _isInitialized;

        public void Initialize(Vector3 position, MagicCircleConfig config)
        {
            _affectedEnemies.Clear();
            
            _config = config;
            _elapsedTime = 0f;
            _tickElapsedTime = 0f;
            _isActivated = false;
            _isInitialized = _config.Owner != null;

            transform.position = position;
            CacheReferences();
            ApplyVisualScale();

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

            _elapsedTime += Time.deltaTime;
            Rotate();

            if (!_isActivated && _elapsedTime >= _config.ActivationDelay)
            {
                _isActivated = true;
                Activate();
            }

            if (_isActivated)
            {
                TickActiveState(Time.deltaTime);
            }

            if (_elapsedTime >= _config.Duration)
            {
                ReturnToPool();
            }
        }

        private void Rotate()
        {
            CacheReferences();

            var target = rotationAxis != null ? rotationAxis : transform;
            target.Rotate(0f, 0f, _config.RotationSpeed * Time.deltaTime);
        }

        private void Activate()
        {
            switch (_config.Mode)
            {
                case EMagicCircleMode.DamageOnce:
                case EMagicCircleMode.AreaDamage:
                    DamageEnemiesInArea(_config.DamageMultiplier);
                    break;
                
                case EMagicCircleMode.Pull:
                    PullEnemiesInArea();
                    break;
            }
        }

        private void TickActiveState(float deltaTime)
        {
            if (_config.TickInterval <= 0f)
            {
                return;
            }
            
            _tickElapsedTime += deltaTime;
            while (_tickElapsedTime >= _config.TickInterval)
            {
                _tickElapsedTime -= _config.TickInterval;
                
                switch (_config.Mode)
                {
                    case EMagicCircleMode.AreaDamage:
                        DamageEnemiesInArea(_config.TickDamageMultiplier);
                        break;
                    
                    case EMagicCircleMode.Pull:
                        PullEnemiesInArea();
                        break;
                }
            }
        }

        private int FindEnemiesInArea()
        {
            CacheReferences();
            if (circleCollider == null)
            {
                DebugExtension.LogColor("MagicCircle: CircleCollider2D is missing.", Color.red);
                return 0;
            }

            var contactFilter = new ContactFilter2D
            {
                useLayerMask = enemyLayer.value != 0,
                layerMask = enemyLayer,
                useTriggers = true
            };

            return circleCollider.Overlap(contactFilter, _overlapResults);
        }

        private void DamageEnemiesInArea(float damageMultiplier)
        {
            if (damageMultiplier <= 0f)
            {
                return;
            }
            
            _affectedEnemies.Clear();
            var count = FindEnemiesInArea();
            for (var index = 0; index < count; index++)
            {
                var collider2d = _overlapResults[index];
                var enemy = collider2d.GetComponentInParent<Enemy>();
                if (!CanDamage(enemy))
                {
                    continue;
                }

                Damage(enemy, damageMultiplier);
            }
        }
        
        private void PullEnemiesInArea()
        {
            if (_config.PullDistance <= 0f)
            {
                return;
            }
            
            _affectedEnemies.Clear();
            var count = FindEnemiesInArea();
            for (var index = 0; index < count; index++)
            {
                var collider2d = _overlapResults[index];
                var enemy = collider2d.GetComponentInParent<Enemy>();
                if (!CanAffect(enemy))
                {
                    continue;
                }

                Pull(enemy);
            }
        }

        private bool CanDamage(Enemy enemy)
        {
            return CanAffect(enemy);
        }
        
        private bool CanAffect(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null &&
                   !enemy.Health.IsDead &&
                   !_affectedEnemies.Contains(enemy);
        }

        private void Damage(Enemy enemy, float damageMultiplier)
        {
            var owner = _config.Owner;
            if (owner == null || owner.Stat == null)
            {
                return;
            }

            _affectedEnemies.Add(enemy);

            var damageContext = owner.Stat.CreateDamageContext(EAttack.Magic, damageMultiplier);
            owner.TryDamagedToEnemy(enemy, damageContext, _config.TriggerOnHitEffect);
        }
        
        private void Pull(Enemy enemy)
        {
            var owner = _config.Owner;
            if (owner == null)
            {
                return;
            }
            
            _affectedEnemies.Add(enemy);

            var center = transform.position;
            var position = enemy.transform.position;
            var direction = center - position;
            direction.z = 0f;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                owner.NotifyAffectedEnemy(enemy);
                return;
            }

            var nextPosition = Vector3.MoveTowards(position, center, _config.PullDistance);
            enemy.transform.position = nextPosition;
            owner.NotifyAffectedEnemy(enemy);
        }

        private void ApplyVisualScale()
        {
            CacheReferences();

            transform.localScale = _baseScale * Mathf.Max(0.01f, _config.VisualScale);
        }

        private void CacheReferences()
        {
            if (rotationAxis == null)
            {
                rotationAxis = transform;
            }

            if (!_hasBaseScale)
            {
                _baseScale = transform.localScale;
                _hasBaseScale = true;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (circleCollider == null)
            {
                circleCollider = GetComponentInChildren<CircleCollider2D>(true);
            }

        }

        private void ReturnToPool()
        {
            _isInitialized = false;
            _affectedEnemies.Clear();
            ObjectPoolManager.Instance.Return(gameObject);
        }

        private void OnDisable()
        {
            _isInitialized = false;
            _affectedEnemies.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var radius = circleCollider != null ? circleCollider.radius : _config.Radius;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}
