namespace PNTD
{
    [HeroSkill(EHeroType.Crusader)]
    public class CrusaderSkill : ClericSkill
    {
        protected override ClericSanctuaryData SanctuaryData { get; } = new()
        {
            radius = 1.5f,
            auraType = EClericAuraType.None,
            levelValues = new[] { 0.18f, 0.24f, 0.30f },
            tickInterval = 0.5f,
            judgedDamageTakenBonusValues = new[] { 0.06f, 0.08f, 0.10f },
        };
    }
}
