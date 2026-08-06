using System.Collections.Generic;
using System.Linq;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public static class HeroContextTooltipUtility
    {
        public static TooltipContent Create(HeroContext heroContext, string mode)
        {
            if (heroContext == null)
            {
                return null;
            }

            return Create(heroContext.HeroDataTableRow, heroContext.Level, mode);
        }

        public static TooltipContent Create(HeroDataTableRow heroDataTableRow, int heroLevel, string mode, bool omitHeroLevel = false, SynergyContext synergyContext = null)
        {
            if (heroDataTableRow == null)
            {
                return null;
            }

            var heroColor = ColorUtility.ToHtmlStringRGB(heroDataTableRow.color);
            return new TooltipContent(
                $"PartySlot_{heroDataTableRow.displayName}",
                new Dictionary<string, object>
                {
                    { "heroName", $"<color=#{heroColor}>{heroDataTableRow.displayName}</color>" },
                    { "heroLevel", heroLevel },
                    { "deployCost", heroDataTableRow.cost },
                    { "heroSynergies", BuildHeroSynergies(heroDataTableRow.synergy) },
                    { "mode", mode },
                    { "omitHeroLevel", omitHeroLevel },
                    { "orbCount", CalculateStarbornOrbCount(heroDataTableRow, heroLevel, synergyContext) },
                }
            );
        }

        public static TooltipContent CreateShopSlot(HeroDataTableRow heroDataTableRow, SynergyContext synergyContext = null)
        {
            if (heroDataTableRow == null)
            {
                return null;
            }
            
            var heroColor = ColorUtility.ToHtmlStringRGB(heroDataTableRow.color);
            return new TooltipContent(
                $"ShopSlot_{heroDataTableRow.displayName}",
                new Dictionary<string, object>
                {
                    { "heroName", $"<color=#{heroColor}>{heroDataTableRow.displayName}</color>" },
                    { "heroTier", (int)heroDataTableRow.tier },
                    { "deployCost", heroDataTableRow.cost },
                    { "heroSynergies", BuildHeroSynergies(heroDataTableRow.synergy) },
                    { "mode", "Buys" },
                    { "orbCount", CalculateStarbornOrbCount(heroDataTableRow, 1, synergyContext) },
                }
            );
        }

        private static int CalculateStarbornOrbCount(HeroDataTableRow heroDataTableRow, int heroLevel, SynergyContext synergyContext)
        {
            if (heroDataTableRow == null || !EnumUtility.HasAnyFlag(heroDataTableRow.synergy, ESynergy.StarBorn))
            {
                return 0;
            }

            return Mathf.Clamp(heroLevel + GetStarbornSynergyOrbCount(synergyContext?.GetCount(ESynergy.StarBorn) ?? 0), 1, 6);
        }

        private static int GetStarbornSynergyOrbCount(int starbornCount)
        {
            if (starbornCount >= 4)
            {
                return 3;
            }

            return starbornCount >= 2 ? 1 : 0;
        }
        
        private static string BuildHeroSynergies(ESynergy synergy)
        {
            if (synergy == ESynergy.None)
            {
                return "None";
            }

            var synergies = EnumUtility.GetValues<ESynergy>()
                .Where(type => type != ESynergy.None && EnumUtility.HasAnyFlag(synergy, type))
                .Select(FormatSynergyName)
                .ToList();

            return synergies.Count > 0 ? string.Join(", ", synergies) : "None";
        }
        
        private static string FormatSynergyName(ESynergy synergyType)
        {
            if (!SynergyDataTableUtility.TryGetSynergyDataTableRow(synergyType, out var synergyDataTableRow) || 
                                                                   synergyDataTableRow == null)
            {
                return synergyType.ToString();
            }

            var synergyColor = ColorUtility.ToHtmlStringRGB(synergyDataTableRow.color);
            return $"<color=#{synergyColor}>{synergyDataTableRow.displayName}</color>";
        }
    }
}
