using System;
using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PNTD
{
    public class ShopSlotView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private Image hoverImage;
        [SerializeField] private HeroInfoView heroInfoView;
        [SerializeField] private SynergySlotView[] synergySlotViews;
        
        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private ShopSlotEffect shopSlotEffect;

        private HeroDataTableRow _heroDataTableRow;
        private int _slotIndex;
        private Vector2 _originAnchoredPosition;

        private Tween _fadeTween;
        private Tween _clickTween;

        public event Action<HeroDataTableRow, int> OnClickedSlot;

        private void Awake()
        {
            _originAnchoredPosition = RectTransform.anchoredPosition;
        }

        public void Initialize(ShopSlotContext shopSlotContext, 
                               SynergyContext synergyContext, 
                               int slotIndex)
        {
            _slotIndex = slotIndex;
            _heroDataTableRow = shopSlotContext.HeroDataTableRow;

            var synergyDataTableRows = shopSlotContext.SynergyDataTableRows;
            var canIncreaseSynergy = shopSlotContext.CanIncreaseSynergy;

            heroInfoView.Initialize(_heroDataTableRow, synergyContext);

            foreach (var synergySlotView in synergySlotViews)
            {
                synergySlotView.StopHighlight();
                synergySlotView.gameObject.SetActive(false);
            }

            if (synergyDataTableRows == null)
            {
                return;
            }

            var loopCount = Mathf.Min(synergyDataTableRows.Count, synergySlotViews.Length);
            for (var index = 0; index < loopCount; index++)
            {
                var synergyDataRow = synergyDataTableRows[index];
                var synergySlotView = synergySlotViews[index];

                if (synergySlotView == null)
                {
                    synergySlotView.gameObject.SetActive(false);
                    continue;
                }
                
                var currentStep = synergyContext?.GetCount(synergyDataRow.synergy) ?? 0;

                synergySlotView.gameObject.SetActive(true);
                synergySlotView.Initialize(synergyDataRow, currentStep);

                if (canIncreaseSynergy && currentStep < synergyDataRow.maxStep)
                {
                    synergySlotView.HighlightNextStep(true);
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySFX("SFX_Hover2");
            
            _fadeTween?.Kill();
            _fadeTween = shopSlotEffect.PlayPointerEnterEffect(hoverImage);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _fadeTween?.Kill();
            _fadeTween = shopSlotEffect.PlayPointerExitEffect(hoverImage);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            
            SoundManager.Instance.PlaySFX("SFX_Click");

            _clickTween?.Kill();
            _clickTween = shopSlotEffect.PlayPointerDownEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _clickTween?.Kill();
            _clickTween = shopSlotEffect.PlayPointerUpEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            
            OnClickedSlot?.Invoke(_heroDataTableRow, _slotIndex);
        }

        private void OnDisable()
        {
            _fadeTween?.Kill();
            _clickTween?.Kill();
            
            RectTransform.anchoredPosition = _originAnchoredPosition;
            hoverImage.color = new Color(hoverImage.color.r, hoverImage.color.g, hoverImage.color.b, 0f);

            foreach (var synergySlotView in synergySlotViews)
            {
                if (synergySlotView == null)
                {
                    continue;
                }
                
                synergySlotView.StopHighlight();
            }
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _clickTween?.Kill();
        }
    }
}
