using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ArtillerySkill : RangerSkill
    {
        protected override float BulletSpeed => 4f;
        protected override float BulletHitRadius => 0.35f;
        protected override float BulletVisualScale => 4f;
        protected override int PierceCount => 5;
        protected override float GetBulletSpeed(Hero hero) => hero != null && hero.Level >= 3 ? BulletSpeed * 2f : BulletSpeed;

        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_Artillery");
        }
        
        protected override void FireBullet(Hero hero, Vector2 direction)
        {
            var bulletPrefab = PrefabManager.CachePrefab<RangerBullet>(BulletPrefabName);
            if (bulletPrefab == null)
            {
                DebugExtension.LogColor($"Ranger Skill: RangerBullet prefab '{BulletPrefabName}' is missing.", Color.red);
                return;
            }

            var bulletObject = ObjectPoolManager.Instance.Get(bulletPrefab.gameObject);
            if (bulletObject == null)
            {
                return;
            }

            var bullet = bulletObject.GetComponent<RangerBullet>();
            if (bullet == null)
            {
                ObjectPoolManager.Instance.Return(bulletObject);
                return;
            }

            var origin = hero.Model != null && hero.Model.RotationAxis != null
                ? hero.Model.RotationAxis.position
                : hero.transform.position;

            var config = new RangerBulletConfig(hero,
                DamageMultiplier,
                GetBulletSpeed(hero),
                hero.Stat.FinalAttackRange,
                GetBulletHitRadius(hero),
                GetPierceCount(hero),
                GetBulletVisualScale(hero));

            bullet.Initialize(origin, direction, config, true);
        }
    }
}
