namespace PNTD
{
    public class HeroStatModifierEffect : HeroEffect
    {
        private readonly EHeroStat _stat;
        private readonly EHeroStatModifierOperation _operation;
        private readonly float _value;

        public HeroStatModifierEffect(EHeroStat stat,
                                      EHeroStatModifierOperation operation,
                                      float value)
        {
            _stat = stat;
            _operation = operation;
            _value = value;
        }

        protected override void OnApply(Hero hero)
        {
            hero?.Stat?.AddModifier(this, _stat, _operation, _value);
        }

        public override void Release(Hero hero)
        {
            hero?.Stat?.RemoveModifiersFrom(this);
        }
    }
}
