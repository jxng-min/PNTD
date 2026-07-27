using System.Collections.Generic;
using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    public class SynergySlotView : MonoBehaviour, ITooltipProvider
    {
        [BigHeader("UI")]
        [SerializeField] private Image synergyImage;
        [SerializeField] private Image[] stepImages;
        
        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private SynergySlotEffect synergySlotEffect;
 
        private static readonly Color32 InactiveStepColor = new(32, 32, 32, 255);

        private SynergyDataTableRow _synergyDataTableRow;
        private int _maxStep;
        private Tween _highlightTween;

        public ESynergy Synergy => _synergyDataTableRow != null ? _synergyDataTableRow.synergy : ESynergy.None;
        public int CurrentStep { get; private set; }
        public bool CanShowTooltip => _synergyDataTableRow != null;

        public void Initialize(SynergyDataTableRow synergyDataTableRow, int currentStep = 0)
        {
            StopHighlight();
            
            _synergyDataTableRow = synergyDataTableRow;
            if (_synergyDataTableRow == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (synergyImage != null)
            {
                synergyImage.sprite = _synergyDataTableRow.icon;
            }
            
            _maxStep = Mathf.Min(stepImages.Length, _synergyDataTableRow.maxStep);
            SetStep(currentStep);
        }

        public void UpdateStep(ESynergy synergy, int currentStep)
        {
            if (_synergyDataTableRow == null || synergy != _synergyDataTableRow.synergy)
            {
                return;
            }
            
            SetStep(currentStep);
        }

        public void SetStep(int currentStep)
        {
            if (_synergyDataTableRow == null)
            {
                return;
            }

            ClearHighlight();
            
            CurrentStep = Mathf.Clamp(currentStep, 0, _maxStep);
            RefreshStepColors();
        }
        
        public void HighlightNextStep(bool isActive)
        {
            HighlightStep(CurrentStep, isActive);
        }

        public void HighlightStep(int step, bool isOn)
        {
            StopHighlight();

            if (!isOn || _synergyDataTableRow == null)
            {
                return;
            }

            if (step < 0 || step >= _maxStep)
            {
                return;
            }
            
            var stepImage = stepImages[step];
            stepImage.color = _synergyDataTableRow.color;

            _highlightTween = synergySlotEffect.PlayHighlightEffect(stepImage);
        }

        public void StopHighlight()
        {
            ClearHighlight();
            RefreshStepColors();
        }
        
        private void ClearHighlight()
        {
            _highlightTween?.Kill();
            _highlightTween = null;
        }

        private void RefreshStepColors()
        {
            if (_synergyDataTableRow == null)
            {
                return;
            }

            for (var step = 0; step < stepImages.Length; step++)
            {
                var stepImage = stepImages[step];
                if (stepImage == null)
                {
                    continue;
                }

                var active = step < _maxStep;
                stepImage.gameObject.SetActive(active);

                if (!active)
                {
                    continue;
                }
                
                RestoreStepColor(step);
            }
        }

        private void RestoreStepColor(int step)
        {
            if (_synergyDataTableRow == null)
            {
                return;
            }

            if (step < 0 || step >= _maxStep)
            {
                return;
            }
            
            var stepImage = stepImages[step];
            if (stepImage == null)
            {
                return;
            }

            Color color = step < CurrentStep ? _synergyDataTableRow.color : InactiveStepColor;
            color.a = 1f;
            stepImage.color = color;
        }

        private void OnDisable()
        {
            StopHighlight();
        }

        private void OnDestroy()
        {
            ClearHighlight();
        }

        public TooltipContent GetTooltipContent()
        {
            if (_synergyDataTableRow == null)
            {
                return null;
            }

            var tooltipID = _synergyDataTableRow.rowID;
            var thresholdText = SynergyDataTableUtility.BuildThresholdText(_synergyDataTableRow, CurrentStep);
            var valueText = SynergyDataTableUtility.BuildValueText(_synergyDataTableRow, CurrentStep);
            var synergyColor = ColorUtility.ToHtmlStringRGB(_synergyDataTableRow.color);

            return new TooltipContent(
                tooltipID,
                new Dictionary<string, object>
                {
                    { "synergyName", $"<color=#{synergyColor}>{_synergyDataTableRow.displayName}</color>" },
                    { "synergyDescription", _synergyDataTableRow.description },
                    { "synergyCount", $"<color=#FFCE1B>{CurrentStep}</color>" },
                    { "thresholdText", thresholdText },
                    { "valueText", valueText },
                    { "synergyTierText", SynergyDataTableUtility.BuildTierText(_synergyDataTableRow, CurrentStep) },
                });
        }
    }
}