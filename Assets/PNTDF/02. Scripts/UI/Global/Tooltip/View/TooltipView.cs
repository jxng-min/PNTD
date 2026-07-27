using DG.Tweening;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public abstract class TooltipView : ViewBase
    {
        [BigHeader("Layout")]
        [SerializeField] private ETooltipLayout layout;

        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private TooltipEffect tooltipEffect;
        
        private Tween _fadeTween;
        
        public ETooltipLayout Layout => layout;

        public void Show(TooltipDataTableRow tooltipDataTableRow,
                         TooltipContent tooltipContent)
        {
            if (tooltipDataTableRow == null || tooltipContent == null)
            {
                return;
            }
            
            gameObject.SetActive(true);
            Bind(tooltipDataTableRow, tooltipContent);
            
            _fadeTween?.Kill();
            _fadeTween = tooltipEffect.PlayShowTooltipEffect(CanvasGroup);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            _fadeTween?.Kill();
            _fadeTween = tooltipEffect.PlayHideTooltipEffect(CanvasGroup, HideImmediate);
        }

        public void HideImmediate()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
            
            gameObject.SetActive(false);
        }
        
        protected abstract void Bind(TooltipDataTableRow tooltipDataTableRow,
                                     TooltipContent tooltipContent);
    }
}