using System;
using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class SynergyPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [SerializeField, SceneOnly, Required] private Transform synergySlotHolder;

        private readonly Dictionary<ESynergy, SynergySlotView> _synergySlotDict = new();
        private SynergySlotView[] _synergySlotViews;
        private SynergyContext _synergyContext = SynergyContext.Empty;

        private void Awake()
        {
            _synergySlotViews = synergySlotHolder.GetComponentsInChildren<SynergySlotView>(true);
            UpdateSynergySlots(null, null);
        }

        public void UpdateSynergySlots(SynergyContext synergyContext,
                                       IReadOnlyList<SynergyDataTableRow> synergyDataTableRows)
        {
            _synergyContext = synergyContext ?? SynergyContext.Empty;

            StopAllHighlights();
            _synergySlotDict.Clear();

            if (synergyDataTableRows == null || synergyDataTableRows.Count == 0)
            {
                HideAllSlots();
                return;
            }
            
            for (var index = 0; index < _synergySlotViews.Length; index++)
            {
                var synergySlotView = _synergySlotViews[index];
                if (synergySlotView == null)
                {
                    continue;
                }

                if (index >= synergyDataTableRows.Count)
                {
                    synergySlotView.gameObject.SetActive(false);
                    continue;
                }

                var synergyDataTableRow = synergyDataTableRows[index];
                if (synergyDataTableRow == null)
                {
                    synergySlotView.gameObject.SetActive(false);
                    continue;
                }

                var synergyCount = _synergyContext.GetCount(synergyDataTableRow.synergy);
                var isActive = synergyCount > 0;

                synergySlotView.gameObject.SetActive(isActive);

                if (!isActive)
                {
                    continue;
                }

                synergySlotView.Initialize(synergyDataTableRow, synergyCount);
                _synergySlotDict[synergyDataTableRow.synergy] = synergySlotView;
            }

            SortSynergySlots();
        }

        public void HighlightHeroSynergies(IReadOnlyList<SynergyDataTableRow> synergyDataTableRows)
        {
            StopAllHighlights();

            if (synergyDataTableRows == null)
            {
                return;
            }

            for (var index = 0; index < synergyDataTableRows.Count; index++)
            {
                var synergyDataTableRow = synergyDataTableRows[index];
                if (synergyDataTableRow == null)
                {
                    continue;
                }
                
                HighlightSynergyLastStep(synergyDataTableRow.synergy);
            }
        }

        public void HighlightHeroSynergies(ESynergy synergy)
        {
            StopAllHighlights();

            if (synergy == ESynergy.None)
            {
                return;
            }

            var eSynergies = EnumUtility.GetValues<ESynergy>();
            foreach (var eSynergy in eSynergies)
            {
                if (eSynergy == ESynergy.None)
                {
                    continue;
                }

                if (!synergy.HasFlag(eSynergy))
                {
                    continue;
                }
                
                HighlightSynergyLastStep(eSynergy);
            }
        }

        public void HighlightHeroSynergies(IReadOnlyList<ESynergy> synergies)
        {
            StopAllHighlights();

            if (synergies == null)
            {
                return;
            }

            foreach (var synergy in synergies)
            {
                HighlightSynergyLastStep(synergy);
            }
        }

        public void HighlightSynergyLastStep(ESynergy synergy)
        {
            if (!_synergySlotDict.TryGetValue(synergy, out var synergySlotView))
            {
                return;
            }

            if (synergySlotView == null)
            {
                return;
            }

            var currentStep = _synergyContext.GetCount(synergy);
            if (currentStep <= 0)
            {
                return;
            }
            
            synergySlotView.HighlightStep(currentStep - 1, true);
        }

        public void StopAllHighlights()
        {
            if (_synergySlotViews == null)
            {
                return;
            }

            foreach (var synergySlotView in _synergySlotViews)
            {
                if (synergySlotView == null)
                {
                    continue;
                }
                
                synergySlotView.StopHighlight();
            }
        }

        private void SortSynergySlots()
        {
            Array.Sort(_synergySlotViews, CompareSynergySlots);

            var index = 0;
            foreach (var synergySlotView in _synergySlotViews)
            {
                if (synergySlotView == null)
                {
                    continue;
                }

                if (!synergySlotView.gameObject.activeSelf)
                {
                    continue;
                }
                
                synergySlotView.transform.SetSiblingIndex(index);
                index++;
            }
        }

        private static int CompareSynergySlots(SynergySlotView lhs, SynergySlotView rhs)
        {
            if (lhs == null && rhs == null)
            {
                return 0;
            }

            if (lhs == null)
            {
                return 1;
            }

            if (rhs == null)
            {
                return -1;
            }

            var xActive = lhs.CurrentStep > 0;
            var yActive = rhs.CurrentStep > 0;

            if (xActive != yActive)
            {
                return yActive.CompareTo(xActive);
            }

            var countCompare = rhs.CurrentStep.CompareTo(lhs.CurrentStep);

            if (countCompare != 0)
            {
                return countCompare;
            }

            return lhs.Synergy.CompareTo(rhs.Synergy);
        }

        private void HideAllSlots()
        {
            if (_synergySlotViews == null)
            {
                return;
            }

            foreach (var synergySlotView in _synergySlotViews)
            {
                if (synergySlotView == null)
                {
                    continue;
                }
                
                synergySlotView.StopHighlight();
                synergySlotView.gameObject.SetActive(false);
            }
        }
    }
}