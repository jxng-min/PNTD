namespace PNTD
{
    public class JovianSkill : StarbornSkill
    {
        protected override StarbornAttackData AttackData { get; } = new()
        {
            orbDamageRatio = 0.32f,
            orbitRadius = 1.0f,
            orbitSpeed = 110f,
            orbRadius = 0.34f,
            rehitInterval = 0.65f,
            orbitShape = EOrbitShape.Circle,
            contactEffect = EStarbornContactEffect.Slow,
            effectId = "JovianGravitySlow",
            effectValue = 0.80f,
            effectDuration = 1f,
        };
    }
}
