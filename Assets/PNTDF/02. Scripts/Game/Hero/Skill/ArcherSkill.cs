namespace PNTD
{
    public class ArcherSkill : RangerSkill
    {
        protected override void PlayFireSound()
        {
            SoundManager.Instance.PlaySFX("SFX_Archer");
        }
    }
}
