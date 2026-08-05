using System.Collections.Generic;

namespace PNTD
{
    public class DeployContext
    {
        public int SlotIndex { get; }
        public HeroDataTableRow HeroDataTableRow { get; }
        public int Level { get; }
        public IReadOnlyList<HeroEffect> Effects { get; }

        public DeployContext(int slotIndex,
                             HeroDataTableRow heroDataTableRow, 
                             int level, 
                             IReadOnlyList<HeroEffect> effects)
        {
            SlotIndex = slotIndex;
            HeroDataTableRow = heroDataTableRow;
            Level = level;
            Effects = effects;
        }
    }
}
