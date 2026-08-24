namespace PNTD
{
    [HeroSkill(EHeroType.Venusian)]
    public class VenusianSkill : StarbornSkill
    {
        protected override StarbornAttackData AttackData { get; } = new()
        {
            orbDamageRatio = 0.30f,
            orbitRadius = 0.75f,
            orbitSpeed = 120f,
            orbRadius = 0.30f,
            rehitInterval = 0.65f,
            orbitShape = EOrbitShape.Circle,
            contactEffect = EStarbornContactEffect.Corrosion,
            effectId = "VenusianCorrosion",
            effectValue = 0.06f,
            effectDuration = 3f,
            effectTickInterval = 0.5f,
        };
    }
}
