namespace PNTD
{
    public class SaintSkill : ClericSkill
    {
        protected override ClericSanctuaryData SanctuaryData { get; } = new()
        {
            radius = 1.5f,
            auraType = EClericAuraType.AttackPower,
            levelValues = new[] { 0.20f, 0.30f, 0.40f },
        };
    }
}
