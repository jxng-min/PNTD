using System;
using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class PalettePresenter : MonoBehaviour
    {
        [SerializeField] private PaletteView paletteView;

        private IReadOnlyList<HeroContext> _heroContexts;
        private DeploySystem _deploySystem;
        private DeployContextFactory _deployContextFactory;
        private Func<SynergyContext> _synergyContextProvider;

        public void Initialize(IReadOnlyList<HeroContext> heroContexts,
                               DeploySystem deploySystem,
                               DeployContextFactory deployContextFactory,
                               Func<SynergyContext> synergyContextProvider)
        {
            Release();

            _heroContexts = heroContexts;
            _deploySystem = deploySystem;
            _deployContextFactory = deployContextFactory;
            _synergyContextProvider = synergyContextProvider;

            paletteView?.Initialize(_heroContexts);

            if (paletteView != null)
            {
                paletteView.OnClickedSlot += HandleOnClickedSlot;
            }
        }

        public void Release()
        {
            if (paletteView != null)
            {
                paletteView.OnClickedSlot -= HandleOnClickedSlot;
            }

            _heroContexts = null;
            _deploySystem = null;
            _deployContextFactory = null;
            _synergyContextProvider = null;
        }

        private void HandleOnClickedSlot(int slotIndex)
        {
            if (_heroContexts == null || slotIndex < 0 || slotIndex >= _heroContexts.Count)
            {
                return;
            }

            if (_deploySystem == null || _deployContextFactory == null)
            {
                return;
            }

            var heroContext = _heroContexts[slotIndex];
            var synergyContext = _synergyContextProvider?.Invoke() ?? SynergyContext.Empty;
            var deployContext = _deployContextFactory.Create(slotIndex, heroContext, synergyContext);

            _deploySystem.EnterDeployMode(deployContext);
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}
