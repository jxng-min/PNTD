using System;
using System.Collections.Generic;

namespace PNTD
{
    public class SynergySystem
    {
        public event Action<SynergyContext> OnSynergyUpdated;

        public SynergyContext CurrentContext { get; private set; } = SynergyContext.Empty;
        public SynergyDataTableRow[] SynergyDataTableRows { get; private set; }

        public SynergySystem(SynergyDataTableRow[] synergyDataTableRows)
        {
            SynergyDataTableRows = synergyDataTableRows;
        }

        public void Initialize(IReadOnlyList<HeroContext> heroContexts)
        {
            RefreshSynergies(heroContexts);
        }

        public void RefreshSynergies(IReadOnlyList<HeroContext> heroContexts)
        {
            if (heroContexts == null)
            {
                CurrentContext = SynergyContext.Empty;
                OnSynergyUpdated?.Invoke(CurrentContext);
                return;
            }
            
            var synergyCounts = SynergyCalculator.Calculate(heroContexts);
            // TODO: 시너지 적용 후 현재 컨텍스트 갱신
            OnSynergyUpdated?.Invoke(CurrentContext);
        }
    }
}