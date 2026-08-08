namespace PNTD
{
    public class BlinkerData : EnemyAbilityData
    {
        public float FirstCastDelay { get; }
        public float CoolDown { get; }
        public float BlinkDistance { get; }
        public float InvincibleDuration { get; }

        public BlinkerData(string abilityID,
                           float firstCastDelay,
                           float coolDown,
                           float blinkDistance,
                           float invincibleDuration)
            : base(abilityID)
        {
            FirstCastDelay = firstCastDelay;
            CoolDown = coolDown;
            BlinkDistance = blinkDistance;
            InvincibleDuration = invincibleDuration;
        }
    }
}
