using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public abstract class WarriorSkill : HeroSkill
    {
        protected const string SpacePrefabName = "[PF] Warrior Space";
        protected const string FanPrefabName = "[PF] Warrior Fan";
        protected const string AxePrefabName = "[PF] Warrior Axe";

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

        protected WarriorSpace SpawnSpace(Vector3 position, Vector2 size, Hero owner, float damageMultiplier)
        {
            var prefab = PrefabManager.CachePrefab<WarriorSpace>(SpacePrefabName);
            if (prefab == null)
            {
                DebugExtension.LogColor($"Warrior Skill: WarriorSpace prefab '{SpacePrefabName}' is missing.", Color.red);
                return null;
            }

            var spaceObject = ObjectPoolManager.Instance.Get(prefab.gameObject);
            if (spaceObject == null)
            {
                return null;
            }

            var space = spaceObject.GetComponent<WarriorSpace>();
            if (space == null)
            {
                ObjectPoolManager.Instance.Return(spaceObject);
                return null;
            }

            space.Initialize(position, size, owner, damageMultiplier);
            return space;
        }

        protected WarriorFan SpawnFan(Hero owner,
                                      float damageMultiplier,
                                      float orbitRadius,
                                      float degreesPerSecond,
                                      int revolutionCount)
        {
            var prefab = PrefabManager.CachePrefab<WarriorFan>(FanPrefabName);
            if (prefab == null)
            {
                DebugExtension.LogColor($"Warrior Skill: WarriorFan prefab '{FanPrefabName}' is missing.", Color.red);
                return null;
            }

            var fanObject = ObjectPoolManager.Instance.Get(prefab.gameObject);
            if (fanObject == null)
            {
                return null;
            }

            var fan = fanObject.GetComponent<WarriorFan>();
            if (fan == null)
            {
                ObjectPoolManager.Instance.Return(fanObject);
                return null;
            }

            fan.Initialize(owner, damageMultiplier, orbitRadius, degreesPerSecond, revolutionCount);
            return fan;
        }

        protected WarriorAxe SpawnAxe(Hero owner,
                                      Vector3 origin,
                                      Vector2 direction,
                                      float damageMultiplier,
                                      float speed,
                                      float maxDistance,
                                      float hitRadius,
                                      bool canReturn)
        {
            var prefab = PrefabManager.CachePrefab<WarriorAxe>(AxePrefabName);
            if (prefab == null)
            {
                DebugExtension.LogColor($"Warrior Skill: WarriorAxe prefab '{AxePrefabName}' is missing.", Color.red);
                return null;
            }

            var axeObject = ObjectPoolManager.Instance.Get(prefab.gameObject);
            if (axeObject == null)
            {
                return null;
            }

            var axe = axeObject.GetComponent<WarriorAxe>();
            if (axe == null)
            {
                ObjectPoolManager.Instance.Return(axeObject);
                return null;
            }

            axe.Initialize(owner, origin, direction, damageMultiplier, speed, maxDistance, hitRadius, canReturn);
            return axe;
        }
    }
}
