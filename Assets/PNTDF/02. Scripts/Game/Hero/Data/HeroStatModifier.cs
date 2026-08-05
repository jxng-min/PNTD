namespace PNTD
{
    public class HeroStatModifier
    {
        public object Source { get; }
        public EHeroStat Stat { get; }
        public EHeroStatModifierOperation Operation { get; }
        public float Value { get; }

        public HeroStatModifier(object source, 
                                EHeroStat stat, 
                                EHeroStatModifierOperation operation, 
                                float value)
        {
            Source = source;
            Stat = stat;
            Operation = operation;
            Value = value;
        }

        public bool IsSource(object source)
        {
            return ReferenceEquals(source, Source);
        }
    }
}