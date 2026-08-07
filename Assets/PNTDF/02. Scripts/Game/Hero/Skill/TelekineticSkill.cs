using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class TelekineticSkill : HeroSkill
    {
        private const string MagicCirclePrefabName = "[PF] Mage Circle";
        
        private const float ActivationDelay = 0f;
        private const float LevelOneDuration = 2.5f;
        private const float LevelThreeDuration = 5f;
        private const float CircleRadius = 2f;
        private const float CircleScale = 1f;
        private const float TickInterval = 0.2f;
        private const float PullDistance = 0.2f;
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
                DebugExtension.LogColor($"Telekinetic Skill: MagicCircle prefab '{MagicCirclePrefabName}' is missing.", Color.red);
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
                                               0f,
                                               CircleRadius,
                                               ActivationDelay,
                                               GetDuration(hero),
                                               RotationSpeed,
                                               CircleScale,
                                               triggerOnHitEffect: false,
                                               mode: EMagicCircleMode.Pull,
                                               tickInterval: TickInterval,
                                               pullDistance: PullDistance);

            circle.Initialize(position, config);
        }

        private static float GetDuration(Hero hero)
        {
            return hero != null && hero.Level >= 3
                ? LevelThreeDuration
                : LevelOneDuration;
        }
    }
}
