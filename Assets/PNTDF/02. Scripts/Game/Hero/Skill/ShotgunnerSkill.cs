namespace PNTD
{
    public class ShotgunnerSkill : RangerSkill
    {
        protected override float DamageMultiplier => 0.55f;
        protected override float BulletHitRadius => 0.14f;
        protected override int GetBulletCount(Hero hero) => 3;
        protected override float GetSpreadAngle(Hero hero) => 45f;
        protected override bool UseFullCircle(Hero hero) => hero != null && hero.Level >= 3;
    }
}
