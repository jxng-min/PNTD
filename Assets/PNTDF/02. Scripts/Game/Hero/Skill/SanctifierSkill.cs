namespace PNTD
{
    [HeroSkill(EHeroType.Sancitifier)]
    public class SanctifierSkill : ClericSkill
    {
        protected override ClericSanctuaryData SanctuaryData { get; } = new()
        {
            radius = 1.5f,
            auraType = EClericAuraType.AttackTempo,
            levelValues = new[] { 0.10f, 0.15f, 0.20f },
        };
    }
}
