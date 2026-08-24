namespace PNTD
{
    [HeroSkill(EHeroType.Magician)]
    public class MagicianSkill : MageSkill
    {
        private const float LevelOneDotDamageMultiplier = 0.1f;
        private const float LevelThreeDotDamageMultiplier = 0.2f;
        
        protected override float DamageMultiplier => 0.8f;
        protected override float ProjectileSpeed => 8f;
        protected override float ProjectileHitRadius => 0.14f;
        protected override string DotEffectId => "MagicianBurn";
        protected override float DotTickInterval => 0.5f;
        protected override float DotDuration => 3f;

        protected override float GetDotDamageMultiplier(Hero hero)
        {
            return hero != null && hero.Level >= 3
                ? LevelThreeDotDamageMultiplier
                : LevelOneDotDamageMultiplier;
        }
        
        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_Magician");
        }
    }
}
