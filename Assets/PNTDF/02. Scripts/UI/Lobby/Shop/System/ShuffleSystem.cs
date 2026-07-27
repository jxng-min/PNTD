using System.Collections.Generic;
using System.Linq;
using JxModule.DataTable;
using JxModule;

namespace PNTD
{
    public class ShuffleSystem
    {
        private readonly DataTable _heroDataTable;
        private readonly DataTable _synergyDataTable;
        private readonly DataTable _shopRateDataTable;

        private readonly Dictionary<ETier, List<HeroDataTableRow>> _heroDict = new();
        private readonly Dictionary<ESynergy, SynergyDataTableRow> _synergyDict = new();
        private readonly Dictionary<int, ShopRateDataTableRow> _shopRateDict = new();
            
#region TierWeight
    private class TierWeight
    {
        public readonly ETier Tier;
        public readonly int Weight;

        public TierWeight(ETier tier, int weight)
        {
            Tier = tier;
            Weight = weight;
        }
    }
#endregion

        public ShuffleSystem(DataTable heroDataTable, 
                             DataTable synergyDataTable, 
                             DataTable shopRateDataTable)
        {
            _heroDataTable = heroDataTable;
            _synergyDataTable = synergyDataTable;
            _shopRateDataTable = shopRateDataTable;
        }

        public void Initialize()
        {
            var heroDataTableRows = _heroDataTable.FindAll<HeroDataTableRow>();
            foreach (var heroDataTableRow in heroDataTableRows)
            {
                if (!_heroDict.TryGetValue(heroDataTableRow.tier, out var list))
                {
                    list = new List<HeroDataTableRow>();
                    _heroDict[heroDataTableRow.tier] = list;
                }
                
                list.Add(heroDataTableRow);
            }
            
            var synergyDataTableRows = _synergyDataTable.FindAll<SynergyDataTableRow>();
            foreach (var synergyDataTableRow in synergyDataTableRows)
            {
                _synergyDict[synergyDataTableRow.synergy] = synergyDataTableRow;
            }
            
            var shopRateDataTableRows = _shopRateDataTable.FindAll<ShopRateDataTableRow>();
            foreach (var shopRateDataTableRow in shopRateDataTableRows)
            {
                _shopRateDict[shopRateDataTableRow.level] = shopRateDataTableRow;
            }
        }

        public ETier GetTier(int level)
        {
            if (!_shopRateDict.TryGetValue(level, out var row))
            {
                return ETier.None;
            }

            var tiers = new List<TierWeight>
            {
                new(ETier.Tier1, row.tier1),
                new(ETier.Tier2, row.tier2),
                new(ETier.Tier3, row.tier3)
            };
            
            var selectedTier = RandomUtility.GetWeightedRandom(tiers, tier => tier.Weight);
            return selectedTier.Tier;
        }

        public HeroDataTableRow GetHeroDataTableRow(ETier tier)
        {
            if (!_heroDict.TryGetValue(tier, out var heroDataTableRows))
            {
                return null;
            }

            if (heroDataTableRows.IsNullOrEmpty())
            {
                return null;
            }
            
            return RandomUtility.GetRandom(heroDataTableRows);
        }

        public List<SynergyDataTableRow> GetSynergyDataTableRows(ESynergy synergy)
        {
            return (from ESynergy eSynergy in System.Enum.GetValues(typeof(ESynergy)) 
                where eSynergy != ESynergy.None 
                where EnumUtility.HasAnyFlag(eSynergy, synergy) 
                select _synergyDict[eSynergy]).ToList();
        }
    }
}