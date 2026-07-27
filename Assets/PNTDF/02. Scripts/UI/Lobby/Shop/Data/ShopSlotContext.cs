using System.Collections.Generic;

namespace PNTD
{
    public readonly struct ShopSlotContext
    {
        public readonly HeroDataTableRow HeroDataTableRow;
        public readonly IReadOnlyList<SynergyDataTableRow> SynergyDataTableRows;
        public readonly bool CanIncreaseSynergy;

        public ShopSlotContext(HeroDataTableRow heroDataTableRow, 
                               IReadOnlyList<SynergyDataTableRow> synergyDataTableRows, 
                               bool canIncreaseSynergy)
        {
            HeroDataTableRow = heroDataTableRow;
            SynergyDataTableRows = synergyDataTableRows;
            CanIncreaseSynergy = canIncreaseSynergy;
        }
    }
}