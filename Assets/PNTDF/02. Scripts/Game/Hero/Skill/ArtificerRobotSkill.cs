using System.Collections;
using UnityEngine;

namespace PNTD
{
    [HeroSkill(EHeroType.MageRobo)]
    public class ArtificerRobotSkill : HeroSkill
    {
        private const float DamageMultiplier = 0.55f;
        private const string SlowEffectId = "ArtificerRobotSlow";
        private const float SlowMultiplier = 0.7f;
        private const float SlowDuration = 1.5f;

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

            var source = hero.Summoner != null ? hero.Summoner : hero;
            if (source.Stat == null)
            {
                yield break;
            }

            var damageContext = source.Stat.CreateDamageContext(EAttack.Magic, DamageMultiplier);
            source.TryDamagedToEnemy(target, damageContext);

            if (source.Level >= 3 && target.Health != null && !target.Health.IsDead)
            {
                target.Status?.AddMoveSpeedEffect(SlowEffectId,
                                                  SlowMultiplier,
                                                  SlowDuration,
                                                  stackPolicy: EStackPolicy.RefreshDuration);
            }
        }
    }
}
