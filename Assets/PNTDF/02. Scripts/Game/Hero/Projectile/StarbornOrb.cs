using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StarbornOrb : MonoBehaviour
    {
        [BigHeader("References")]
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private readonly Dictionary<Enemy, float> _nextHitTimes = new();

        private StarbornOrbitController _controller;
        private int _orbIndex;
        private bool _isInitialized;

        public void Initialize(StarbornOrbitController controller, int orbIndex)
        {
            _controller = controller;
            _orbIndex = orbIndex;
            _isInitialized = _controller != null;
            _nextHitTimes.Clear();

            CacheReferences();
            ApplyVisuals();

            if (!_isInitialized)
            {
                ReturnToPool();
            }
        }

        public void Refresh(int orbIndex)
        {
            _orbIndex = orbIndex;
            ApplyVisuals();
        }

        public void SetCollisionEnabled(bool isEnabled)
        {
            CacheReferences();

            if (circleCollider != null)
            {
                circleCollider.enabled = isEnabled;
            }
        }

        public void SetOrbitPosition(float angle, float radius)
        {
            var radians = angle * Mathf.Deg2Rad;
            transform.localPosition = new Vector3(Mathf.Cos(radians) * radius,
                                                  Mathf.Sin(radians) * radius,
                                                  0f);
        }

        public void SetOrbitPosition(float angle, float radiusX, float radiusY)
        {
            var radians = angle * Mathf.Deg2Rad;
            transform.localPosition = new Vector3(Mathf.Cos(radians) * radiusX,
                                                  Mathf.Sin(radians) * radiusY,
                                                  0f);
        }

        public void ReturnToPool()
        {
            _isInitialized = false;
            _controller = null;
            _nextHitTimes.Clear();
            ObjectPoolManager.Instance.Return(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHit(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryHit(other);
        }

        private void TryHit(Collider2D other)
        {
            if (!_isInitialized || _controller == null || !_controller.CanDamage)
            {
                return;
            }

            var enemy = other != null ? other.GetComponentInParent<Enemy>() : null;
            if (!CanHit(enemy))
            {
                return;
            }

            var now = Time.time;
            if (_nextHitTimes.TryGetValue(enemy, out var nextHitTime) && now < nextHitTime)
            {
                return;
            }

            _nextHitTimes[enemy] = now + _controller.Data.rehitInterval;
            Hit(enemy);
        }

        private bool CanHit(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null &&
                   !enemy.Health.IsDead;
        }

        private void Hit(Enemy enemy)
        {
            var hero = _controller.Owner;
            if (hero == null || hero.Stat == null || enemy == null)
            {
                return;
            }

            var damageContext = hero.Stat.CreateDamageContext(EAttack.Magic, _controller.Data.orbDamageRatio);
            hero.TryDamagedToEnemy(enemy, damageContext, false);

            if (!enemy.Health.IsDead)
            {
                ApplyContactEffect(hero, enemy);
            }

            hero.NotifyHitEnemy(enemy);
            hero.NotifyAffectedEnemy(enemy);
        }

        private void ApplyContactEffect(Hero hero, Enemy enemy)
        {
            var data = _controller.Data;
            if (enemy.Status == null)
            {
                return;
            }

            switch (data.contactEffect)
            {
                case EStarbornContactEffect.Corrosion:
                    var damagePerTick = hero.Stat.FinalMagicAttackPower * data.effectValue;
                    enemy.Status.AddDoTEffect(data.effectId,
                                              damagePerTick,
                                              data.effectTickInterval,
                                              data.effectDuration,
                                              hero.Stat.FinalFlatMagicPenetration,
                                              hero.Stat.FinalPercentMagicPenetration,
                                              EAttack.Magic);
                    break;

                case EStarbornContactEffect.Slow:
                    enemy.Status.AddMoveSpeedEffect(data.effectId,
                                                    data.effectValue,
                                                    data.effectDuration,
                                                    stackPolicy: EStackPolicy.RefreshDuration);
                    break;
            }
        }

        private void ApplyVisuals()
        {
            if (_controller == null)
            {
                return;
            }

            CacheReferences();

            if (circleCollider != null)
            {
                circleCollider.enabled = true;
                circleCollider.isTrigger = true;
                circleCollider.radius = _controller.Data.orbRadius;
            }

            if (spriteRenderer != null && _controller.Owner?.HeroDataTableRow != null)
            {
                spriteRenderer.color = _controller.Owner.HeroDataTableRow.color;
            }

            gameObject.name = $"Starborn Orb {_orbIndex + 1}";
        }

        private void CacheReferences()
        {
            if (circleCollider == null)
            {
                circleCollider = GetComponentInChildren<CircleCollider2D>(true);
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }
        }

        private void OnDisable()
        {
            _isInitialized = false;
            _nextHitTimes.Clear();
        }
    }
}
