using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class MinerSkill : RangerSkill
    {
        private const int GoldAmount = 1;
        private const float GoldMinSpawnOffset = 0.20f;
        private const float GoldMaxSpawnOffset = 0.40f;

        private int _miningProgress;

        protected override float DamageMultiplier => 0.70f;
        protected override float BulletSpeed => 9f;

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || !TryGetFireDirection(hero, out var direction))
            {
                yield break;
            }

            IncreaseMiningProgress(hero);
            yield return FireBullets(hero, direction);
        }

        public override void Release(Hero hero)
        {
            base.Release(hero);
            _miningProgress = 0;
        }

        private void IncreaseMiningProgress(Hero hero)
        {
            _miningProgress++;
            RefreshMiningProgress(hero);
        }

        private void RefreshMiningProgress(Hero hero)
        {
            var requiredAttackCount = GetRequiredAttackCount(hero);
            while (_miningProgress >= requiredAttackCount)
            {
                _miningProgress -= requiredAttackCount;
                SpawnGold(hero);
            }
        }

        private static int GetRequiredAttackCount(Hero hero)
        {
            if (hero == null)
            {
                return 5;
            }

            return hero.Level switch
            {
                2 => 4,
                3 => 3,
                _ => 5,
            };
        }

        private void SpawnGold(Hero hero)
        {
            if (hero == null)
            {
                return;
            }

            Context?.GoldSpawner?.Spawn(hero.transform.position,
                                        GoldAmount,
                                        GoldMinSpawnOffset,
                                        GoldMaxSpawnOffset);
        }
    }
}
