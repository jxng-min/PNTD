using System;
using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PNTD
{
    public class IndexerSlotView : LabelBoxView
    {
        [BigHeader("Effect")]
        [SerializeField] private IndexerSlotEffect indexerSlotEffect;

        private int _slotIndex;
        private bool _isFocusing;
        private Vector2 _originAnchoredPosition;
        private Vector2 _originScale;
        
        private Tween _hoverTween;
        private Tween _clickTween;
        
        public event Action<int> OnClickedIndexerSlot;

        private void Awake()
        {
            _originAnchoredPosition = RectTransform.anchoredPosition;
        }

        public void Initialize(int slotIndex)
        {
            _slotIndex = slotIndex;
        }

        public void SetFocus(bool isFocusing)
        {
            _isFocusing = isFocusing;
            if (isFocusing)
            {
                return;
            }
            
            _hoverTween?.Kill();
            _hoverTween = indexerSlotEffect.PlayPointerExitEffect(Image);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            _hoverTween?.Kill();
            _hoverTween = indexerSlotEffect.PlayPointerEnterEffect(Image);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_isFocusing)
            {
                return;
            }
            
            _hoverTween?.Kill();
            _hoverTween = indexerSlotEffect.PlayPointerExitEffect(Image);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            _clickTween?.Kill();
            _clickTween = indexerSlotEffect.PlayPointerDownEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _clickTween?.Kill();
            _clickTween = indexerSlotEffect.PlayPointerUpEffect(RectTransform, _originAnchoredPosition);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            
            OnClickedIndexerSlot?.Invoke(_slotIndex);
        }
    }
}