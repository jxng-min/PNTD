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
    }
}