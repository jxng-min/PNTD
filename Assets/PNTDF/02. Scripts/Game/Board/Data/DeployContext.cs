using System.Collections.Generic;

namespace PNTD
{
    public class DeployContext
    {
        public HeroDataTableRow HeroDataTableRow { get; }
        public int Level { get; }
        public IReadOnlyList<HeroEffect> Effects { get; }

        public DeployContext(HeroDataTableRow heroDataTableRow, 
                             int level, 
                             IReadOnlyList<HeroEffect> effects)
        {
            HeroDataTableRow = heroDataTableRow;
            Level = level;
            Effects = effects;
        }
    }
}