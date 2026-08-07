using System;
using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class PaletteView : ViewBase
    {
        private PaletteSlotView[] _paletteSlotViews;
        private bool _isBound;

        public event Action<int> OnClickedSlot;

        private void Awake()
        {
            CacheSlotViews();
        }

        public void Initialize(IReadOnlyList<HeroContext> heroContexts)
        {
            CacheSlotViews();
            HideAllSlots();

            if (heroContexts == null || _paletteSlotViews == null)
            {
                return;
            }
            
            var loopCount = Mathf.Min(heroContexts.Count, _paletteSlotViews.Length);
            for (var index = 0; index < loopCount; index++)
            {
                var heroContext = heroContexts[index];
                if (heroContext == null || heroContext.HeroDataTableRow == null)
                {
                    continue;
                }
                
                _paletteSlotViews[index].gameObject.SetActive(true);
                _paletteSlotViews[index].Initialize(index, heroContext);
            }
        }

        public void UpdateSlotState(string heroId, bool isUsing)
        {
            if (_paletteSlotViews == null)
            {
                return;
            }

            foreach (var paletteSlotView in _paletteSlotViews)
            {
                paletteSlotView.UpdateState(heroId, isUsing);
            }
        }

        public void UpdateSlotState(int slotIndex, bool isUsing)
        {
            if (_paletteSlotViews == null || slotIndex < 0 || slotIndex >= _paletteSlotViews.Length)
            {
                return;
            }

            _paletteSlotViews[slotIndex].UpdateState(isUsing);
        }

        public void UpdateSlotState(bool isUsing)
        {
            if (_paletteSlotViews == null)
            {
                return;
            }

            foreach (var paletteSlotView in _paletteSlotViews)
            {
                paletteSlotView.UpdateState(isUsing);
            }
        }

        private void HandleOnClickedSlot(int slotId)
        {
            OnClickedSlot?.Invoke(slotId);
        }

        private void HideAllSlots()
        {
            if (_paletteSlotViews == null)
            {
                return;
            }

            foreach (var paletteSlotView in _paletteSlotViews)
            {
                paletteSlotView.gameObject.SetActive(false);
            }
        }

        private void CacheSlotViews()
        {
            if (_isBound)
            {
                return;
            }

            _paletteSlotViews = GetComponentsInChildren<PaletteSlotView>(true);
            foreach (var paletteSlotView in _paletteSlotViews)
            {
                if (paletteSlotView == null)
                {
                    continue;
                }

                paletteSlotView.OnClickedSlot += HandleOnClickedSlot;
            }

            _isBound = true;
        }

        private void OnDestroy()
        {
            if (_paletteSlotViews == null)
            {
                return;
            }

            foreach (var paletteSlotView in _paletteSlotViews)
            {
                if (paletteSlotView == null)
                {
                    continue;
                }

                paletteSlotView.OnClickedSlot -= HandleOnClickedSlot;
            }

            _isBound = false;
        }
    }
}
