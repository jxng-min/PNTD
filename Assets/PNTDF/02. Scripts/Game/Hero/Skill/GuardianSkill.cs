using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class GuardianSkill : RangerSkill
    {
        protected override float DamageMultiplier => 0.55f;
        protected override float BulletSpeed => 8.5f;

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || !TryGetFireDirection(hero, out var upDirection))
            {
                yield break;
            }

            var directions = GetDirections(upDirection);
            var repeatCount = hero.Level >= 3 ? 3 : 1;
            for (var repeat = 0; repeat < repeatCount; repeat++)
            {
                foreach (var direction in directions)
                {
                    FireBullet(hero, direction);
                    PlayFireSound();
                }

                if (repeat + 1 < repeatCount)
                {
                    yield return new WaitForSeconds(0.12f);
                }
            }
        }

        private static Vector2[] GetDirections(Vector2 upDirection)
        {
            var up = upDirection.sqrMagnitude > Mathf.Epsilon ? upDirection.normalized : Vector2.up;
            var right = new Vector2(up.y, -up.x);

            return new[]
            {
                up,
                right,
                -up,
                -right,
            };
        }
    }
}
