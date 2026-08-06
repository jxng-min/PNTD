namespace PNTD
{
    public class MartianSkill : StarbornSkill
    {
        protected override StarbornAttackData AttackData { get; } = new()
        {
            orbDamageRatio = 0.45f,
            orbitRadius = 0.75f,
            orbitSpeed = 135f,
            orbRadius = 0.30f,
            rehitInterval = 0.60f,
            orbitShape = EOrbitShape.Circle,
        };
    }
}
