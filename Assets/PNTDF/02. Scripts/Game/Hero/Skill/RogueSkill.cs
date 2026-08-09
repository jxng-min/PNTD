using JxModule;
using UnityEngine;

namespace PNTD
{
    public abstract class RogueSkill : HeroSkill
    {
        protected const string RogueCrowPrefabName = "[PF] Rogue Crow";
        protected const string RogueDaggerPrefabName = "[PF] Rogue Dagger";

        protected HeroSkillContext Context { get; private set; }

        public override void Initialize(HeroSkillContext context)
        {
            Context = context;
        }

        protected static Vector3 GetAttackOrigin(Hero hero)
        {
            return hero != null && hero.Model != null && hero.Model.RotationAxis != null
                ? hero.Model.RotationAxis.position
                : hero != null
                    ? hero.transform.position
                    : Vector3.zero;
        }

        protected RogueCrow SpawnRogueCrow(Hero owner,
                                           Enemy target,
                                           float damageMultiplier,
                                           float speed,
                                           float hitDistance)
        {
            var prefab = PrefabManager.CachePrefab<RogueCrow>(RogueCrowPrefabName);
            if (prefab == null)
            {
                DebugExtension.LogColor($"Rogue Skill: RogueCrow prefab '{RogueCrowPrefabName}' is missing.", Color.red);
                return null;
            }

            var projectileObject = ObjectPoolManager.Instance.Get(prefab.gameObject);
            if (projectileObject == null)
            {
                return null;
            }

            var projectile = projectileObject.GetComponent<RogueCrow>();
            if (projectile == null)
            {
                ObjectPoolManager.Instance.Return(projectileObject);
                return null;
            }

            projectile.Initialize(GetAttackOrigin(owner), owner, target, damageMultiplier, speed, hitDistance);
            return projectile;
        }

        protected RogueDagger SpawnRogueDagger(Hero owner,
                                               Vector3 origin,
                                               Vector2 direction,
                                               RogueDaggerConfig config)
        {
            var prefab = PrefabManager.CachePrefab<RogueDagger>(RogueDaggerPrefabName);
            if (prefab == null)
            {
                DebugExtension.LogColor($"Rogue Skill: RogueDagger prefab '{RogueDaggerPrefabName}' is missing.", Color.red);
                return null;
            }

            var daggerObject = ObjectPoolManager.Instance.Get(prefab.gameObject);
            if (daggerObject == null)
            {
                return null;
            }

            var dagger = daggerObject.GetComponent<RogueDagger>();
            if (dagger == null)
            {
                ObjectPoolManager.Instance.Return(daggerObject);
                return null;
            }

            dagger.Initialize(origin, direction, config);
            return dagger;
        }

        protected static Vector2 Rotate(Vector2 direction, float angle)
        {
            var radians = angle * Mathf.Deg2Rad;
            var sin = Mathf.Sin(radians);
            var cos = Mathf.Cos(radians);

            return new Vector2(
                direction.x * cos - direction.y * sin,
                direction.x * sin + direction.y * cos
            ).normalized;
        }
    }
}
