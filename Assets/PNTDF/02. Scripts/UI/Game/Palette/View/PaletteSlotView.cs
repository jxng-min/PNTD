using System;
using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PNTD
{
    public class PaletteSlotView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private Image hoverImage;
        [SerializeField] private Image heroImage;
        [SerializeField] private Image usageImage;
        
        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private PaletteSlotEffect paletteSlotEffect;

        private HeroContext _heroContext;
        private Vector2 _originAnchoredPosition;
        private int _slotIndex;
        private bool _isUsing;

        private Tween _hoverTween;
        private Tween _clickTween;

        public event Action<int> OnClickedSlot;

        private void Awake()
        {
            _originAnchoredPosition = RectTransform.anchoredPosition;
        }
        
        public void Initialize(int slotIndex, HeroContext heroContext)
        {
            if(heroContext == null || heroContext.HeroDataTableRow == null)
            {
                return;
            }
            
            _heroContext = heroContext;
            _slotIndex = slotIndex;
            
            heroImage.color = _heroContext.HeroDataTableRow.color;
            usageImage.color = Color.green;
        }
        
        public void UpdateState(string heroId, bool isUsing)
        {
            if(!gameObject.activeInHierarchy)
            {
                return;
            }
            
            if(_heroContext == null || _heroContext.HeroDataTableRow == null)
            {
                return;
            }
            
            if(heroId != _heroContext.HeroDataTableRow.rowID)
            {
                return;
            }
            
            SetUsingState(isUsing);
        }
        
        public void UpdateState(bool isUsing)
        {
            if(!gameObject.activeInHierarchy)
            {
                return;
            }
            
            SetUsingState(isUsing);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            _hoverTween?.Kill();
            _hoverTween = paletteSlotEffect.PlayPointerEnterEffect(hoverImage);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _hoverTween?.Kill();
            _hoverTween = paletteSlotEffect.PlayPointerExitEffect(hoverImage);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (_isUsing)
            {
                return;
            }
            
            _clickTween?.Kill();
            _clickTween = paletteSlotEffect.PlayPointerDownEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _clickTween?.Kill();
            _clickTween = paletteSlotEffect.PlayPointerUpEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (_isUsing)
            {
                return;
            }
            
            OnClickedSlot?.Invoke(_slotIndex);
        }

        private void SetUsingState(bool isUsing)
        {
            _isUsing = isUsing;
            usageImage.color = isUsing ? Color.red : Color.green;
        }

        private void OnDisable()
        {
            _heroContext = null;
            _slotIndex = -1;
            _isUsing = false;
            
            _hoverTween?.Kill();
            _hoverTween = null;
            
            _clickTween?.Kill();
            _clickTween = null;
            
            hoverImage.transform.localScale = Vector3.one;
            heroImage.color = Color.white;
            usageImage.color = Color.green;
            
            RectTransform.anchoredPosition = _originAnchoredPosition;
        }
    }
}