using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class SwordsmanSkill : WarriorSkill
    {
        private static readonly Vector2 SpaceSize = new(1.3f, 1.3f);

        public override IEnumerator Execute(Hero hero)
        {
            var target = hero?.Caster?.FindNearestTarget();
            if (target == null)
            {
                yield break;
            }

            SpawnSpace(target.transform.position, SpaceSize, hero, GetDamageMultiplier(hero));
        }

        private static float GetDamageMultiplier(Hero hero)
        {
            return hero != null
                ? hero.Level switch
            {
                2 => 1.10f,
                3 => 1.20f,
                _ => 1f,
            }
                : 1f;
        }
    }
}
