namespace PNTD
{
    public class HandgunnerSkill : RangerSkill
    {
        protected override float DamageMultiplier => 0.4f;
        protected override int GetBulletCount(Hero hero) => hero != null && hero.Level >= 3 ? 8 : 3;
        
        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_HandShot");
        }
    }
}
