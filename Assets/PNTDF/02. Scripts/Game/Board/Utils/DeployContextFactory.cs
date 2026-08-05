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
            if (heroContext.Level < 3)
            {
                return;
            }
        }

        private void AddSynergyEffects(HeroContext heroContext,
                                       SynergyContext synergyContext,
                                       List<HeroEffect> effects)
        {
        }
    }
}
