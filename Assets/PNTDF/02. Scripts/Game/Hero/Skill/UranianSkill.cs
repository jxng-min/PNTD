namespace PNTD
{
    public class UranianSkill : StarbornSkill
    {
        protected override StarbornAttackData AttackData { get; } = new()
        {
            orbDamageRatio = 0.36f,
            orbitSpeed = 400f,
            orbRadius = 0.28f,
            rehitInterval = 0.60f,
            orbitShape = EOrbitShape.Ellipse,
            ellipseRadiusX = 0.75f,
            ellipseRadiusY = 0.5f,
            distributeEllipseRotationByOrb = true,
        };
    }
}
