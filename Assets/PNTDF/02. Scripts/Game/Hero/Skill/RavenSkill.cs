using System.Collections;

namespace PNTD
{
    public class RavenSkill : RogueSkill
    {
        private const float BaseDamageMultiplier = 1f;
        private const float LevelTwoDamageMultiplier = 1.2f;
        private const float ProjectileSpeed = 9f;
        private const float HitDistance = 0.15f;

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || hero.Caster == null)
            {
                yield break;
            }

            var targets = hero.Caster.FindLowestCurrentHpTargets(GetTargetCount(hero));
            foreach (var target in targets)
            {
                SpawnRogueCrow(hero, target, GetDamageMultiplier(hero), ProjectileSpeed, HitDistance);
            }
        }

        private static int GetTargetCount(Hero hero)
        {
            return hero != null && hero.Level >= 3 ? 3 : 1;
        }

        private static float GetDamageMultiplier(Hero hero)
        {
            return hero != null && hero.Level >= 2 ? LevelTwoDamageMultiplier : BaseDamageMultiplier;
        }
    }
}
