using System.Collections;
using System.Linq;
using UnityEngine;

namespace PNTD
{
    public class PaladinSkill : WarriorSkill
    {
        private const int TargetCount = 2;
        private const float DamageMultiplier = 0.35f;
        private const float SpaceInterval = 0.3f;
        private const float SpaceSpawnDelay = 0.15f;
        private static readonly Vector2 SpaceSize = new(0.6f, 0.6f);

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || hero.Caster == null)
            {
                yield break;
            }

            var heroPosition = GetAttackOrigin(hero);
            var targets = hero.Caster.FindTargetsInRange()
                .Where(enemy => enemy != null && enemy.isActiveAndEnabled && enemy.Health != null && !enemy.Health.IsDead)
                .GroupBy(enemy => enemy)
                .Select(group => group.Key)
                .OrderBy(enemy => ((Vector2)(enemy.transform.position - heroPosition)).sqrMagnitude)
                .Take(TargetCount)
                .ToArray();

            if (targets.Length <= 0)
            {
                yield break;
            }

            yield return SpawnSpaces(hero, targets);
        }

        private IEnumerator SpawnSpaces(Hero hero, Enemy[] targets)
        {
            var startPosition = (Vector2)targets[0].transform.position;
            var direction = GetSpaceDirection(targets);
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                yield break;
            }

            direction.Normalize();
            var spaceCount = hero.Level >= 3 ? 6 : 3;

            for (var index = 0; index < spaceCount; index++)
            {
                var position = startPosition + direction * (SpaceInterval * index);
                SpawnSpace(position, SpaceSize, hero, DamageMultiplier);

                if (index + 1 < spaceCount)
                {
                    yield return new WaitForSeconds(SpaceSpawnDelay);
                }
            }
        }

        private static Vector2 GetSpaceDirection(Enemy[] targets)
        {
            if (targets.Length >= 2)
            {
                var direction = targets[1].transform.position - targets[0].transform.position;
                if (direction.sqrMagnitude > Mathf.Epsilon)
                {
                    return direction;
                }
            }

            var target = targets[0];
            if (target?.Movement != null)
            {
                var moveDirection = target.Movement.TargetPosition - target.transform.position;
                if (moveDirection.sqrMagnitude > Mathf.Epsilon)
                {
                    return moveDirection;
                }
            }

            return Vector2.zero;
        }
    }
}
