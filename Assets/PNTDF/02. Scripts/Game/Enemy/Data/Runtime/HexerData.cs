namespace PNTD
{
    public class HexerData : EnemyAbilityData
    {
        public float FirstCastDelay { get; }
        public float CoolDown { get; }
        public float CastRange { get; }
        public float Duration { get; }
        public float AttackDamagePenalty { get; }
        public float AttackSpeedPenalty { get; }
        public int MaxActiveHexPerHexer { get; }

        public HexerData(string abilityID,
                         float firstCastDelay,
                         float coolDown,
                         float castRange,
                         float duration,
                         float attackDamagePenalty,
                         float attackSpeedPenalty,
                         int maxActiveHexPerHexer)
            : base(abilityID)
        {
            FirstCastDelay = firstCastDelay;
            CoolDown = coolDown;
            CastRange = castRange;
            Duration = duration;
            AttackDamagePenalty = attackDamagePenalty;
            AttackSpeedPenalty = attackSpeedPenalty;
            MaxActiveHexPerHexer = maxActiveHexPerHexer;
        }
    }
}
