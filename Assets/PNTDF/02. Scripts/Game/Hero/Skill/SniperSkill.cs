namespace PNTD
{
    public class SniperSkill : RangerSkill
    {
        protected override float BulletSpeed => 18f;
        protected override int PierceCount => 5;
        
        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_Sniper");
        }
    }
}