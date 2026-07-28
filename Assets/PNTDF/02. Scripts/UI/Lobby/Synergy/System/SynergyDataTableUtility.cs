using System.Collections.Generic;
using System.Linq;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public readonly struct SynergyTierContent
    {
        public SynergyTierContent(int threshold,
                                  ESynergyEffect effect,
                                  float primaryValue,
                                  float secondaryValue,
                                  string tierDescription = null)
        {
            Threshold = threshold;
            Effect = effect;
            PrimaryValue = primaryValue;
            SecondaryValue = secondaryValue;
            TierDescription = tierDescription;
        }
        
        public int Threshold { get; }
        public ESynergyEffect Effect { get; }
        public float PrimaryValue { get; }
        public float SecondaryValue { get; }
        public string TierDescription { get; }
    }
    
    public static class SynergyDataTableUtility
    {
        private const string ActiveColor = "#FFFFFF";
        private const string InactiveColor = "#808080";
        
        public static IReadOnlyList<SynergyDataTableRow> GetSynergyDataTableRows()
        {
            return DataTableManager.FindAllRows<SynergyDataTableRow>()
                .Where(row => row != null && row.isEnable)
                .OrderBy(row => row.synergy)
                .ToList();
        }

        public static bool TryGetSynergyDataTableRow(ESynergy synergy,
                                                     out SynergyDataTableRow synergyDataTableRow)
        {
            synergyDataTableRow = GetSynergyDataTableRows()
                .FirstOrDefault(candidate => candidate.synergy == synergy);
            
            return synergyDataTableRow != null;
        }

        public static IReadOnlyList<SynergyTierContent> GetTierContents(SynergyDataTableRow synergyDataTableRow)
        {
            if (synergyDataTableRow == null)
            {
                return System.Array.Empty<SynergyTierContent>();
            }

            var synergyTierDataTableRows = DataTableManager.FindAllRows<SynergyTierDataTableRow>()
                .Where(row => row != null && row.isEnable && row.synergy == synergyDataTableRow.synergy)
                .OrderBy(row => row.threshold)
                .Select(row => new SynergyTierContent(row.threshold,
                                                      row.effect,
                                                      row.primaryValue,
                                                      row.secondaryValue,
                                                      row.tierDescription))
                .ToList();

            if (synergyTierDataTableRows.Count > 0)
            {
                return synergyTierDataTableRows;
            }
            
            return GetLegacyTiers(synergyDataTableRow);
        }

        private static IReadOnlyList<SynergyTierContent> GetLegacyTiers(SynergyDataTableRow synergyDataTableRow)
        {
            var thresholdCount = synergyDataTableRow.thresholds?.Count ?? 0;
            var effectCount = synergyDataTableRow.synergyEffects?.Count ?? 0;
            var primaryValueCount = synergyDataTableRow.primaryValues?.Count ?? 0;
            var secondaryValueCount = synergyDataTableRow.secondaryValues?.Count ?? 0;
            var tierCount = Mathf.Min(Mathf.Min(thresholdCount, effectCount), Mathf.Min(primaryValueCount, secondaryValueCount));
            
            var synergyTierContents = new List<SynergyTierContent>();
            for (var index = 0; index < tierCount; index++)
            {
                synergyTierContents.Add(new SynergyTierContent(synergyDataTableRow.thresholds[index],
                                                               synergyDataTableRow.synergyEffects[index],
                                                               synergyDataTableRow.primaryValues[index],
                                                               synergyDataTableRow.secondaryValues[index]));
            }
            
            return synergyTierContents;
        }
        
        public static string BuildTierText(SynergyDataTableRow row, int currentCount)
        {
            var tiers = GetTierContents(row).OrderBy(tier => tier.Threshold).ToList();
            if (tiers.Count == 0)
            {
                return string.Empty;
            }

            var lines = new List<string>(tiers.Count);
            foreach (var tier in tiers)
            {
                var active = currentCount >= tier.Threshold;
                var body = BuildTierDescription(tier);

                lines.Add(FormatTierLine($"{tier.Threshold}: {body}", active));
            }

            return string.Join("\n", lines);
        }
        
        public static string BuildThresholdText(SynergyDataTableRow synergyDataTableRow, int currentCount)
        {
            var activeThreshold = GetActiveThreshold(synergyDataTableRow, currentCount);
            
            return string.Join("/", GetTierContents(synergyDataTableRow)
                .OrderBy(tier => tier.Threshold)
                .Select(tier => FormatTierValue(tier.Threshold.ToString(), tier.Threshold == activeThreshold)));
        }
        
        public static string BuildValueText(SynergyDataTableRow synergyDataTableRow, int currentCount)
        {
            var activeThreshold = GetActiveThreshold(synergyDataTableRow, currentCount);
            
            return string.Join("/", GetTierContents(synergyDataTableRow)
                .OrderBy(tier => tier.Threshold)
                .Select(tier => FormatTierValue(FormatDisplayValue(tier), tier.Threshold == activeThreshold)));
        }
        
        private static int GetActiveThreshold(SynergyDataTableRow synergyDataTableRow, int currentCount)
        {
            var activeThreshold = 0;
            foreach (var tier in GetTierContents(synergyDataTableRow).OrderBy(tier => tier.Threshold))
            {
                if (currentCount < tier.Threshold)
                {
                    break;
                }

                activeThreshold = tier.Threshold;
            }

            return activeThreshold;
        }
        
        public static string FormatDisplayValue(SynergyTierContent synergyTierContent)
        {
            switch (synergyTierContent.Effect)
            {
                case ESynergyEffect.AttackCooldownMultiplier:
                case ESynergyEffect.EnemySlowOnHit:
                    return $"{Mathf.RoundToInt((1f - synergyTierContent.PrimaryValue) * 100f)}%";

                case ESynergyEffect.EnemyGoldDropOnKill:
                    return $"{Mathf.RoundToInt(synergyTierContent.PrimaryValue * 100f)}%";

                case ESynergyEffect.OraRangeMultiplier:
                case ESynergyEffect.OraEffectMultiplier:
                    return $"{synergyTierContent.PrimaryValue:0.#}x";
            }

            return synergyTierContent.PrimaryValue.ToString("0.#");
        }
        
        private static string BuildTierDescription(SynergyTierContent synergyTierContent)
        {
            if (!string.IsNullOrWhiteSpace(synergyTierContent.TierDescription))
            {
                return synergyTierContent.TierDescription;
            }

            switch (synergyTierContent.Effect)
            {
                case ESynergyEffect.AttackCooldownMultiplier:
                    return $"Reduces attack cooldown by {FormatDisplayValue(synergyTierContent)}";

                case ESynergyEffect.EnemySlowOnHit:
                    return $"Slows enemies by {FormatDisplayValue(synergyTierContent)}";

                case ESynergyEffect.EnemyGoldDropOnKill:
                    return $"Defeated enemies have a {FormatDisplayValue(synergyTierContent)} chance to drop gold";

                case ESynergyEffect.OraRangeMultiplier:
                    return $"Increases the effect radius by {FormatDisplayValue(synergyTierContent)}";

                case ESynergyEffect.OraEffectMultiplier:
                    return $"Doubles the applied effect value";
            }

            return FormatDisplayValue(synergyTierContent);
        }
        
        private static string FormatTierValue(string value, bool active)
        {
            return $"<color={(active ? ActiveColor : InactiveColor)}>{value}</color>";
        }

        private static string FormatTierLine(string value, bool active)
        {
            return $"<color={(active ? ActiveColor : InactiveColor)}>{value}</color>";
        }
    }
}