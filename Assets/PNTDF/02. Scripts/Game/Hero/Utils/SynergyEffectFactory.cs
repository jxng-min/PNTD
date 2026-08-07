using System.Collections.Generic;
using System.Linq;
using JxModule;

namespace PNTD
{
    public static class SynergyEffectFactory
    {
        public static void AddEffects(HeroContext heroContext,
                                      SynergyContext synergyContext,
                                      List<HeroEffect> effects)
        {
            if (heroContext?.HeroDataTableRow == null || synergyContext == null || effects == null)
            {
                return;
            }

            foreach (var synergy in EnumUtility.GetValues<ESynergy>())
            {
                if (synergy == ESynergy.None || !EnumUtility.HasAnyFlag(heroContext.HeroDataTableRow.synergy, synergy))
                {
                    continue;
                }

                var activeTier = GetActiveTier(synergy, synergyContext.GetCount(synergy));
                if (activeTier == null)
                {
                    continue;
                }

                var effect = CreateEffect(activeTier.Value);
                if (effect != null)
                {
                    effects.Add(effect);
                }
            }
        }

        private static SynergyTierContent? GetActiveTier(ESynergy synergy, int count)
        {
            if (!SynergyDataTableUtility.TryGetSynergyDataTableRow(synergy, out var synergyDataTableRow))
            {
                return null;
            }

            SynergyTierContent? activeTier = null;
            foreach (var tier in SynergyDataTableUtility.GetTierContents(synergyDataTableRow).OrderBy(tier => tier.Threshold))
            {
                if (count < tier.Threshold)
                {
                    break;
                }

                activeTier = tier;
            }

            return activeTier;
        }

        private static HeroEffect CreateEffect(SynergyTierContent tier)
        {
            return tier.Effect switch
            {
                ESynergyEffect.AttackCooldownMultiplier =>
                    new HeroStatModifierEffect(EHeroStat.AttackCooldown,
                                               EHeroStatModifierOperation.Multiply,
                                               tier.PrimaryValue),
                
                ESynergyEffect.EnemySlowOnHit =>
                    new MageSynergySlowEffect(tier.PrimaryValue, tier.SecondaryValue),

                ESynergyEffect.StarbornOrbCountBonus =>
                    new StarbornOrbCountEffect((int)tier.PrimaryValue),

                _ => null
            };
        }
    }
}
