namespace PNTD
{
    public class SaturnianSkill : StarbornSkill
    {
        protected override StarbornAttackData AttackData { get; } = new()
        {
            orbDamageRatio = 0.38f,
            orbitRadius = 0.75f,
            secondaryOrbitRadius = 1f,
            orbitSpeed = 105f,
            orbRadius = 0.30f,
            rehitInterval = 0.60f,
            orbitShape = EOrbitShape.PulsingCircle,
            innerHoldDuration = 1f,
            outerHoldDuration = 1f,
            orbitTransitionDuration = 0.5f,
        };
    }
}
