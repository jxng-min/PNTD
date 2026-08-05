using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class REffectContext
    {
        public SynergyContext SynergyContext { get; }
        public IReadOnlyList<HeroContext> Party { get; }
        public Transform Root { get; }

        public REffectContext(SynergyContext synergyContext, 
                              IReadOnlyList<HeroContext> party, 
                              Transform root)
        {
            SynergyContext = synergyContext ?? SynergyContext.Empty;
            Party = party;
            Root = root;
        }
    }
}