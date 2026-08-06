namespace PNTD
{
    public class StarbornOrbCountEffect : HeroEffect
    {
        private readonly int _bonusOrbCount;

        public StarbornOrbCountEffect(int bonusOrbCount)
        {
            _bonusOrbCount = bonusOrbCount;
        }

        protected override void OnApply(Hero hero)
        {
            if (hero?.Skill is StarbornSkill starbornSkill)
            {
                starbornSkill.SetSynergyOrbBonus(_bonusOrbCount);
            }
        }

        public override void Release(Hero hero)
        {
            if (hero?.Skill is StarbornSkill starbornSkill)
            {
                starbornSkill.SetSynergyOrbBonus(0);
            }
        }
    }
}
