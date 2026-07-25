using System;
using System.Collections.Generic;

namespace PNTD
{
    public class SynergyContext
    {
        public static SynergyContext Empty { get; } = new SynergyContext(Array.Empty<HeroContext>(),
                                                                         new Dictionary<ESynergy, int>());
        
        public IReadOnlyList<HeroContext> HeroContexts { get; }
        public IReadOnlyDictionary<ESynergy, int> Counts { get; }

        public SynergyContext(IReadOnlyList<HeroContext> heroContexts,
                              IReadOnlyDictionary<ESynergy, int> counts)
        {
            HeroContexts = heroContexts;
            Counts = counts;
        }
        
        public int GetCount(ESynergy synergy)
        {
            return Counts != null && Counts.TryGetValue(synergy, out var count) ? count : 0;
        }
    }
}