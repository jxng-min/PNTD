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
    }
}
