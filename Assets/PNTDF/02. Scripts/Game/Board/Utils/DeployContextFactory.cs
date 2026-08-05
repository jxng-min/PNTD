using System.Collections.Generic;

namespace PNTD
{
    public class DeployContextFactory
    {
        public DeployContext Create(int slotIndex, HeroContext heroContext, SynergyContext synergyContext)
        {
            if (heroContext == null || heroContext.HeroDataTableRow == null)
            {
                return null;
            }

            var effects = new List<HeroEffect>();

            AddIntrinsicEffects(heroContext, effects);
            AddLevelUnlockEffects(heroContext, effects);
            AddSynergyEffects(heroContext, synergyContext, effects);

            return new DeployContext(slotIndex,
                                     heroContext.HeroDataTableRow,
                                     heroContext.Level,
                                     effects);
        }

        private void AddIntrinsicEffects(HeroContext heroContext, List<HeroEffect> effects)
        {
        }

        private void AddLevelUnlockEffects(HeroContext heroContext, List<HeroEffect> effects)
        {
            switch (heroContext.HeroDataTableRow.rowID)
            {
                case "Hero_Handgunner" when heroContext.Level > 1:
                    effects.Add(new HeroStatModifierEffect(EHeroStat.PhysicalAttackPower,
                                                           EHeroStatModifierOperation.Flat,
                                                           5f * (heroContext.Level - 1)));
                    break;
            }

            if (heroContext.Level < 3)
            {
                return;
            }

            switch (heroContext.HeroDataTableRow.rowID)
            {
                case "Hero_Archer":
                    effects.Add(new HeroStatModifierEffect(EHeroStat.AttackCooldown,
                                                           EHeroStatModifierOperation.Multiply,
                                                           0.70f));
                    break;
                
                case "Hero_Sniper":
                    effects.Add(new HeroStatModifierEffect(EHeroStat.AttackCooldown,
                                                           EHeroStatModifierOperation.Multiply,
                                                           0.50f));
                    break;
            }
        }

        private void AddSynergyEffects(HeroContext heroContext,
                                       SynergyContext synergyContext,
                                       List<HeroEffect> effects)
        {
            SynergyEffectFactory.AddEffects(heroContext, synergyContext, effects);
        }
    }
}
