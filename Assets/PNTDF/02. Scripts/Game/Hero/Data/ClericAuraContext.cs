namespace PNTD
{
    public readonly struct ClericAuraContext
    {
        public EClericAuraType AuraType { get; }
        public Hero Source { get; }
        public float Value { get; }

        public ClericAuraContext(EClericAuraType auraType, Hero source, float value)
        {
            AuraType = auraType;
            Source = source;
            Value = value;
        }
    }
}
