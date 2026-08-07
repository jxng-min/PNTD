using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public abstract class MageSkill : HeroSkill
    {
        protected virtual string ProjectilePrefabName => "[PF] Mage Sphere";
        protected virtual float DamageMultiplier => 1f;
        protected virtual float ProjectileSpeed => 8f;
        protected virtual float ProjectileHitRadius => 0.12f;
        protected virtual string DotEffectId => null;
        protected virtual float DotTickInterval => 0f;
        protected virtual float DotDuration => 0f;
        protected virtual float GetDotDamageMultiplier(Hero hero) => 0f;
        protected virtual string DisableEffectId => null;
        protected virtual float GetDisableChance(Hero hero) => 0f;
        protected virtual float DisableDuration => 0f;
        protected virtual Color? DisableOverrideColor => null;
        protected virtual string StunEffectId => null;
        protected virtual float GetStunDuration(Hero hero) => 0f;
        protected virtual Color? StunOverrideColor => null;

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

            FireProjectile(hero, direction);
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

        protected void FireProjectile(Hero hero, Vector2 direction)
        {
            var projectilePrefab = PrefabManager.CachePrefab<MageProjectile>(ProjectilePrefabName);
            if (projectilePrefab == null)
            {
                DebugExtension.LogColor($"Mage Skill: MageProjectile prefab '{ProjectilePrefabName}' is missing.", Color.red);
                return;
            }

            var projectileObject = ObjectPoolManager.Instance.Get(projectilePrefab.gameObject);
            if (projectileObject == null)
            {
                return;
            }

            var projectile = projectileObject.GetComponent<MageProjectile>();
            if (projectile == null)
            {
                ObjectPoolManager.Instance.Return(projectileObject);
                return;
            }

            var origin = hero.Model != null && hero.Model.RotationAxis != null
                ? hero.Model.RotationAxis.position
                : hero.transform.position;

            var config = new MageProjectileConfig(hero,
                                                  DamageMultiplier,
                                                  ProjectileSpeed,
                                                  hero.Stat.FinalAttackRange,
                                                  ProjectileHitRadius,
                                                  DotEffectId,
                                                  GetDotDamageMultiplier(hero),
                                                  DotTickInterval,
                                                  DotDuration,
                                                  DisableEffectId,
                                                  GetDisableChance(hero),
                                                  DisableDuration,
                                                  DisableOverrideColor,
                                                  StunEffectId,
                                                  GetStunDuration(hero),
                                                  StunOverrideColor);

            projectile.Initialize(origin, direction, config);
        }
    }
}
