namespace PNTD
{
    public enum EOrbitShape
    {
        Circle = 0,
        PulsingCircle = 1,
        Ellipse = 2,
    }

    public enum EStarbornContactEffect
    {
        None = 0,
        Corrosion = 1,
        Slow = 2,
    }

    public class StarbornAttackData
    {
        public float orbDamageRatio;
        public float orbitRadius;
        public float orbitSpeed;
        public float orbRadius;
        public float rehitInterval;
        public EOrbitShape orbitShape;

        public float secondaryOrbitRadius;
        public float orbitTransitionDuration;
        public float innerHoldDuration;
        public float outerHoldDuration;

        public float ellipseRadiusX;
        public float ellipseRadiusY;
        public bool alignEllipseToTarget;
        public bool distributeEllipseRotationByOrb;

        public EStarbornContactEffect contactEffect;
        public string effectId;
        public float effectValue;
        public float effectDuration;
        public float effectTickInterval;
    }
}
