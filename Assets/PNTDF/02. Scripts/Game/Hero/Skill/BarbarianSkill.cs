using System.Collections;

namespace PNTD
{
    [HeroSkill(EHeroType.Barbarian)]
    public class BarbarianSkill : WarriorSkill
    {
        private const float DamageMultiplier = 0.65f;
        private const float OrbitRadius = 0.7f;
        private const float DegreesPerSecond = 360f;

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null)
            {
                yield break;
            }

            SpawnFan(hero, DamageMultiplier, OrbitRadius, DegreesPerSecond, GetRevolutionCount(hero));
            SoundManager.Instance.PlaySFX("SFX_Barbarian");
        }

        private static int GetRevolutionCount(Hero hero)
        {
            return hero != null ? hero.Level : 1;
        }
    }
}
