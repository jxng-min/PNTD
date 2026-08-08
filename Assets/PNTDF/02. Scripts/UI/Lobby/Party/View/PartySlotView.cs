using System;
using DG.Tweening;
using JxModule;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PNTD
{
    public class PartySlotView : ViewBase, IBeginDragHandler, IDragHandler, IEndDragHandler, ITooltipProvider
    {
        private static readonly Color32 InactiveColor = new(80, 80, 80, 255);
        
        [BigHeader("UI")]
        [SerializeField] private Image hoverImage;
        [SerializeField] private Image heroImage;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private Image[] stepImages;
        
        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private PartySlotEffect partySlotEffect;

        private RectTransform _hoverImageRect;
        private Vector2 _originAnchoredPosition;
        private Vector2 _originScale;
        private bool _isDragging;
        
        private Tween _hoverTween;
        private Tween _clickTween;
        private Tween _moveTween;
        
        public event Action<HeroContext, int> OnSlotClicked;
        public event Action<PartySlotView, PointerEventData> OnSlotBeginDrag;
        public event Action<PartySlotView, PointerEventData> OnSlotDrag;
        public event Action<PartySlotView, PointerEventData> OnSlotEndDrag;
        
        public HeroContext HeroContext { get; private set; }
        public bool CanShowTooltip => HeroContext != null && !_isDragging;

        private void Awake()
        {
            _hoverImageRect = hoverImage.GetComponent<RectTransform>();
            _originAnchoredPosition = _hoverImageRect.anchoredPosition;
            _originScale = _hoverImageRect.localScale;
        }

        private void OnEnable()
        {
            var color = hoverImage.color;
            color.a = 0f;
            hoverImage.color = color;
        }

        public void Initialize(HeroContext heroContext)
        {
            HeroContext = heroContext;
            if (HeroContext == null || HeroContext.HeroDataTableRow == null)
            {
                ClearPartySlot();
                return;
            }

            var heroDataTableRow = HeroContext.HeroDataTableRow;
            gameObject.SetActive(true);

            if (heroImage != null)
            {
                heroImage.color = heroDataTableRow.color;
            }

            if (levelLabel != null)
            {
                levelLabel.text = $"{HeroContext.Level}";
            }
            
            UpdateExp(HeroContext.Exp, heroDataTableRow.color);
        }

        public void RefreshPartySlot()
        {
            Initialize(HeroContext);
        }

        public void SetOriginPosition(Vector2 originAnchoredPosition)
        {
            _originAnchoredPosition = originAnchoredPosition;
        }

        public void TweenOriginPosition(Vector2 originAnchoredPosition)
        {
            _originAnchoredPosition = originAnchoredPosition;
            
            _moveTween?.Kill();
            _moveTween = partySlotEffect.PlaySortingEffect(RectTransform, originAnchoredPosition);
        }

        public void ClearPartySlot()
        {
            HeroContext = null;
            
            if (heroImage != null)
            {
                heroImage.color = Color.white;
            }

            if (levelLabel != null)
            {
                levelLabel.text = string.Empty;    
            }

            foreach (var stepImage in stepImages)
            {
                if (stepImage == null)
                {
                    continue;
                }
                
                stepImage.gameObject.SetActive(false);
            }
        }

        private void UpdateExp(int exp, Color color)
        {
            var clampedExp = Mathf.Clamp(exp, 0, stepImages.Length);

            for (var index = 0; index < stepImages.Length; index++)
            {
                var stepImage = stepImages[index];
                if (stepImage == null)
                {
                    continue;
                }

                stepImage.gameObject.SetActive(true);

                var isActive = index < clampedExp;
                stepImage.color = isActive ? color : InactiveColor;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (HeroContext == null)
            {
                return;
            }
            
            _hoverTween?.Kill();
            _hoverTween = partySlotEffect.PlayPointerEnterEffect(RectTransform, hoverImage);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (HeroContext == null)
            {
                return;
            }
            
            _hoverTween?.Kill();
            _hoverTween = partySlotEffect.PlayPointerExitEffect(RectTransform, _originScale, hoverImage);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }
            
            _clickTween?.Kill();
            _clickTween = partySlotEffect.PlayPointerDownEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _clickTween?.Kill();
            _clickTween = partySlotEffect.PlayPointerUpEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right || HeroContext == null)
            {
                return;
            }

            SoundManager.Instance.PlaySFX("SFX_Coin");
            OnSlotClicked?.Invoke(HeroContext, (int)HeroContext.HeroDataTableRow.tier);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || HeroContext == null)
            {
                return;
            }
            
            _isDragging = true;
            
            _hoverTween?.Kill();
            _clickTween?.Kill();
            _moveTween?.Kill();
            
            OnSlotBeginDrag?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }
            
            OnSlotDrag?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }
            
            _isDragging = false;
            OnSlotEndDrag?.Invoke(this, eventData);
        }

        public TooltipContent GetTooltipContent()
        {
            return HeroContextTooltipUtility.Create(HeroContext, "Sells");
        }
    }
}