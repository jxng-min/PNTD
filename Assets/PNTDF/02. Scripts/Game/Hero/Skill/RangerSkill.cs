using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public abstract class RangerSkill : HeroSkill
    {
        private const string BulletPrefabName = "[PF] Ranger Bullet";

        protected virtual float DamageMultiplier => 1f;
        protected virtual float BulletSpeed => 10f;
        protected virtual float BulletHitRadius => 0.12f;
        protected virtual float BulletVisualScale => 1f;
        protected virtual int PierceCount => 1;
        protected virtual int GetBulletCount(Hero hero) => 1;
        protected virtual float GetBulletSpeed(Hero hero) => BulletSpeed;
        protected virtual float GetBulletHitRadius(Hero hero) => BulletHitRadius;
        protected virtual float GetBulletVisualScale(Hero hero) => BulletVisualScale;
        protected virtual int GetPierceCount(Hero hero) => PierceCount;
        protected virtual float GetSpreadAngle(Hero hero) => 0f;
        protected virtual bool UseFullCircle(Hero hero) => false;

        protected HeroSkillContext Context { get; private set; }

        public override void Initialize(HeroSkillContext context)
        {
            Context = context;
        }

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || !TryGetFireDirection(hero, out var direction))
            {
                yield break;
            }

            CoroutineRunner.Instance.Run(FireBullets(hero, direction));
        }

        protected bool TryGetFireDirection(Hero hero, out Vector2 direction)
        {
            direction = default;

            var target = hero?.Caster?.FindNearestTarget();
            if (target == null)
            {
                return false;
            }

            direction = target.transform.position - hero.transform.position;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return false;
            }

            direction.Normalize();
            return true;
        }

        protected IEnumerator FireBullets(Hero hero, Vector2 direction)
        {
            var bulletCount = Mathf.Max(1, GetBulletCount(hero));

            if (UseFullCircle(hero))
            {
                FireFullCircle(hero, direction, bulletCount);
                yield break;
            }

            var spreadAngle = GetSpreadAngle(hero);
            if (bulletCount <= 1 || spreadAngle <= 0f)
            {
                for (var index = 0; index < bulletCount; index++)
                {
                    FireBullet(hero, direction);
                    yield return new WaitForSeconds(0.15f);
                }

                yield break;
            }

            var startAngle = -spreadAngle * 0.5f;
            var stepAngle = spreadAngle / (bulletCount - 1);
            for (var index = 0; index < bulletCount; index++)
            {
                FireBullet(hero, Rotate(direction, startAngle + stepAngle * index));
            }
        }

        private void FireFullCircle(Hero hero, Vector2 direction, int bulletCount)
        {
            var baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var stepAngle = 360f / bulletCount;

            for (var index = 0; index < bulletCount; index++)
            {
                var angle = baseAngle + stepAngle * index;
                FireBullet(hero, AngleToDirection(angle));
            }
        }

        private void FireBullet(Hero hero, Vector2 direction)
        {
            var bulletPrefab = PrefabManager.CachePrefab<RangerBullet>(BulletPrefabName);
            if (bulletPrefab == null)
            {
                DebugExtension.LogColor($"Ranger Skill: RangerBullet prefab '{BulletPrefabName}' is missing.", Color.red);
                return;
            }

            var bulletObject = ObjectPoolManager.Instance.Get(bulletPrefab.gameObject);
            if (bulletObject == null)
            {
                return;
            }

            var bullet = bulletObject.GetComponent<RangerBullet>();
            if (bullet == null)
            {
                ObjectPoolManager.Instance.Return(bulletObject);
                return;
            }

            var origin = hero.Model != null && hero.Model.RotationAxis != null
                ? hero.Model.RotationAxis.position
                : hero.transform.position;

            var config = new RangerBulletConfig(hero,
                                                DamageMultiplier,
                                                GetBulletSpeed(hero),
                                                hero.Stat.FinalAttackRange,
                                                GetBulletHitRadius(hero),
                                                GetPierceCount(hero),
                                                GetBulletVisualScale(hero));

            bullet.Initialize(origin, direction, config);
        }

        private static Vector2 Rotate(Vector2 direction, float angle)
        {
            var radians = angle * Mathf.Deg2Rad;
            var sin = Mathf.Sin(radians);
            var cos = Mathf.Cos(radians);

            return new Vector2(
                direction.x * cos - direction.y * sin,
                direction.x * sin + direction.y * cos
            ).normalized;
        }

        private static Vector2 AngleToDirection(float angle)
        {
            var radians = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }
    }
}
