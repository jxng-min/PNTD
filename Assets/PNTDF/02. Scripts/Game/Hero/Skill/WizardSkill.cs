using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class WizardSkill : HeroSkill
    {
        private const string MagicCirclePrefabName = "[PF] Mage Circle";
        
        private const float LevelOneDamageMultiplier = 0.45f;
        private const float LevelThreeDamageMultiplier = 0.3f;
        private const float ActivationDelay = 0.2f;
        private const float CircleDuration = 0.8f;
        private const float CircleRadius = 0.65f;
        private const float CircleScale = 0.4f;
        private const float MinSpawnRadius = 0.15f;
        private const float MaxSpawnRadius = 0.3f;
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

            CreateMagicCircles(hero, target.transform.position);
        }

        private void CreateMagicCircles(Hero hero, Vector3 targetPosition)
        {
            var circleCount = GetCircleCount(hero);
            var damageMultiplier = GetDamageMultiplier(hero);
            var baseAngle = Random.Range(0f, 360f);
            var angleStep = 360f / circleCount;

            for (var index = 0; index < circleCount; index++)
            {
                var angle = baseAngle + angleStep * index;
                var spawnRadius = Random.Range(MinSpawnRadius, MaxSpawnRadius);
                var position = targetPosition + (Vector3)(AngleToDirection(angle) * spawnRadius);

                CreateMagicCircle(hero, position, damageMultiplier);
            }
        }

        private void CreateMagicCircle(Hero hero, Vector3 position, float damageMultiplier)
        {
            var circlePrefab = PrefabManager.CachePrefab<MagicCircle>(MagicCirclePrefabName);
            if (circlePrefab == null)
            {
                DebugExtension.LogColor($"Wizard Skill: MagicCircle prefab '{MagicCirclePrefabName}' is missing.", Color.red);
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
                                               damageMultiplier,
                                               CircleRadius,
                                               ActivationDelay,
                                               CircleDuration,
                                               RotationSpeed,
                                               CircleScale);

            circle.Initialize(position, config);
        }

        private static int GetCircleCount(Hero hero)
        {
            return hero != null && hero.Level >= 3 ? 8 : 3;
        }

        private static float GetDamageMultiplier(Hero hero)
        {
            return hero != null && hero.Level >= 3
                ? LevelThreeDamageMultiplier
                : LevelOneDamageMultiplier;
        }

        private static Vector2 AngleToDirection(float angle)
        {
            var radians = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }
    }
}
