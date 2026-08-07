namespace PNTD
{
    public class TrickshooterSkill : RangerSkill
    {
        protected override float BulletSpeed => 16f;
        protected override float BulletHitRadius => 0.1f;
        protected override int GetPierceCount(Hero hero) => hero != null && hero.Level >= 3 ? 3 : 1;
        
        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_Trickshooter");
        }
    }
}
