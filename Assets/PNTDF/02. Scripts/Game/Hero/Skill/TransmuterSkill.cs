using UnityEngine;

namespace PNTD
{
    public class TransmuterSkill : MageSkill
    {
        private const float LevelOneDisableChance = 0.3f;
        private const float LevelTwoDisableChance = 0.4f;
        private const float LevelThreeDisableChance = 0.5f;
        
        protected override float DamageMultiplier => 0.7f;
        protected override float ProjectileSpeed => 8f;
        protected override float ProjectileHitRadius => 0.14f;
        protected override string DisableEffectId => "TransmuterAbilitySeal";
        protected override float DisableDuration => 2f;
        protected override Color? DisableOverrideColor => Color.gray;
        protected override string StunEffectId => "TransmuterStun";
        protected override Color? StunOverrideColor => new Color(0.25f, 0.25f, 0.25f, 1f);

        protected override float GetDisableChance(Hero hero)
        {
            if (hero == null)
            {
                return 0f;
            }

            return hero.Level switch
            {
                >= 3 => LevelThreeDisableChance,
                2 => LevelTwoDisableChance,
                _ => LevelOneDisableChance
            };
        }

        protected override float GetStunDuration(Hero hero)
        {
            return hero != null && hero.Level >= 3 ? 0.75f : 0f;
        }

        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_Transmuter");
        }
    }
}
