using System.Collections;
using UnityEngine;

namespace PNTD
{
    [HeroSkill(EHeroType.Thief)]
    public class ThiefSkill : RogueSkill
    {
        private const float DamageMultiplier = 0.45f;
        private const float DaggerSpeed = 14f;
        private const float HitRadius = 0.15f;
        private const float BleedDamageMultiplier = 0.08f;
        private const float BleedTickInterval = 0.5f;
        private const float BleedDuration = 1f;
        private const int LevelOneTwoDaggerCount = 3;
        private const int LevelThreeDaggerCount = 8;
        private const float SpreadAngle = 90f;

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || !TryGetBaseDirection(hero, out var baseDirection))
            {
                yield break;
            }

            var origin = GetAttackOrigin(hero);
            var daggerCount = GetDaggerCount(hero);
            var config = new RogueDaggerConfig(hero,
                                               DamageMultiplier,
                                               DaggerSpeed,
                                               hero.Stat.FinalAttackRange,
                                               HitRadius,
                                               BleedDamageMultiplier,
                                               BleedTickInterval,
                                               BleedDuration);

            if (hero.Level >= 3)
            {
                FireFullCircle(hero, origin, baseDirection, daggerCount, config);
                yield break;
            }

            FireSpread(hero, origin, baseDirection, daggerCount, config);
        }

        private static int GetDaggerCount(Hero hero)
        {
            return hero != null && hero.Level >= 3 ? LevelThreeDaggerCount : LevelOneTwoDaggerCount;
        }

        private bool TryGetBaseDirection(Hero hero, out Vector2 direction)
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

        private void FireSpread(Hero hero,
                                Vector3 origin,
                                Vector2 baseDirection,
                                int daggerCount,
                                RogueDaggerConfig config)
        {
            var startAngle = -SpreadAngle * 0.5f;
            var stepAngle = daggerCount > 1 ? SpreadAngle / (daggerCount - 1) : 0f;

            for (var index = 0; index < daggerCount; index++)
            {
                SpawnRogueDagger(hero, origin, Rotate(baseDirection, startAngle + stepAngle * index), config);
            }
        }

        private void FireFullCircle(Hero hero,
                                    Vector3 origin,
                                    Vector2 baseDirection,
                                    int daggerCount,
                                    RogueDaggerConfig config)
        {
            var stepAngle = 360f / daggerCount;
            for (var index = 0; index < daggerCount; index++)
            {
                SpawnRogueDagger(hero, origin, Rotate(baseDirection, stepAngle * index), config);
            }
        }
    }
}
