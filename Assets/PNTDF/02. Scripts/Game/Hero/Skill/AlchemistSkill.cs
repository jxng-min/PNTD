namespace PNTD
{
    [HeroSkill(EHeroType.Alchemist)]
    public class AlchemistSkill : ClericSkill
    {
        protected override ClericSanctuaryData SanctuaryData { get; } = new()
        {
            radius = 1.5f,
            auraType = EClericAuraType.AdaptivePenetration,
            levelValues = new[] { 5f, 9f, 13f },
        };
    }
}
