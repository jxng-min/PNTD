using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class SlayerSkill : WarriorSkill
    {
        private const float DamageMultiplier = 0.80f;
        private const float AxeSpeed = 7f;
        private const float HitRadius = 0.35f;

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

            var origin = GetAttackOrigin(hero);
            var direction = target.transform.position - origin;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                yield break;
            }

            SpawnAxe(hero,
                     origin,
                     direction.normalized,
                     DamageMultiplier,
                     AxeSpeed,
                     hero.Stat.FinalAttackRange,
                     HitRadius,
                     hero.Level >= 3);
            
            SoundManager.Instance.PlaySFX("SFX_Slayer");
        }
    }
}
