using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ExplomancerSkill : HeroSkill
    {
        private const string MagicCirclePrefabName = "[PF] Mage Circle";
        
        private const float InitialDamageMultiplier = 0.7f;
        private const float TickDamageMultiplier = 0.25f;
        private const float ActivationDelay = 0f;
        private const float CircleDuration = 4f;
        private const float CircleRadius = 1.3f;
        private const float CircleScale = 0.75f;
        private const float LevelOneTickInterval = 1f;
        private const float LevelThreeTickInterval = 0.5f;
        private const float RotationSpeed = 240f;

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null)
            {
                yield break;
            }

            var target = hero.Caster?.FindNearestTarget();
            if (target == null)
            {
                yield break;
            }

            CreateMagicCircle(hero, target.transform.position);
        }

        private void CreateMagicCircle(Hero hero, Vector3 position)
        {
            var circlePrefab = PrefabManager.CachePrefab<MagicCircle>(MagicCirclePrefabName);
            if (circlePrefab == null)
            {
                DebugExtension.LogColor($"Explomancer Skill: MagicCircle prefab '{MagicCirclePrefabName}' is missing.", Color.red);
                return;
            }

            var circleObject = ObjectPoolManager.Instance.Get(circlePrefab.gameObject);
            if (circleObject == null)
            {
                return;
            }

            var circle = circleObject.GetComponent<MagicCircle>();
            if (circle == null)
            {
                ObjectPoolManager.Instance.Return(circleObject);
                return;
            }

            var config = new MagicCircleConfig(hero,
                                               InitialDamageMultiplier,
                                               CircleRadius,
                                               ActivationDelay,
                                               CircleDuration,
                                               RotationSpeed,
                                               CircleScale,
                                               mode: EMagicCircleMode.AreaDamage,
                                               tickDamageMultiplier: TickDamageMultiplier,
                                               tickInterval: GetTickInterval(hero));

            circle.Initialize(position, config);
        }

        private static float GetTickInterval(Hero hero)
        {
            return hero != null && hero.Level >= 3
                ? LevelThreeTickInterval
                : LevelOneTickInterval;
        }
    }
}
