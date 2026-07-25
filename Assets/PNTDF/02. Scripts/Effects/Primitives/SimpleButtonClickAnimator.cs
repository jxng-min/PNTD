using System.Collections;
using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Primitive", "Simple Button", 0)]
    public class SimpleButtonClickAnimator : JxPrimitiveAnimator
    {
        [BigHeader("UI")]
        [SerializeField, Required] private Image targetImage;
        
#region Settings
        [Space(30f)]
        [BigHeader("Settings")]
        [Header("Punch")]
        [SerializeField] private float punchScaleAmount = 0.1f;
        [SerializeField] private float punchDuration = 0.5f;
        
        [Header("Highlight")]
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private float highlightDuration = 0.2f;
        
        [Header("Translation")]
        [SerializeField] private float yOffset = 10f;
        [SerializeField] private float translationDuration = 0.1f;
#endregion
        
        private Tween _hoverTween;
        private Tween _clickTween;

        private RectTransform _rectTransform;
        private Vector2 _originAnchoredPosition;
        private Color _originColor;
        
        private void Awake()
        {
            _rectTransform = targetImage.transform as RectTransform;
            if (_rectTransform == null)
            {
                enabled = false;
                return;
            }
            
            _originAnchoredPosition = _rectTransform.anchoredPosition;
            _originColor = targetImage.color;    
        }
        
        public override void OnPointerEnter()
        {
            _hoverTween?.Kill();
            
            var sequence = DOTween.Sequence();
            
            sequence.Append(
                targetImage.transform.DOPunchScale(
                    new Vector3(punchScaleAmount, punchScaleAmount, 0f), 
                    punchDuration)
            );
            
            sequence.Join(
                targetImage.DOColor(highlightColor, highlightDuration)
            );

            _hoverTween = sequence;
        }

        public override void OnPointerExit()
        {
            _hoverTween?.Kill();
            _hoverTween = targetImage.DOColor(_originColor, highlightDuration);
        }

        public override IEnumerator OnPointerClick()
        {
            yield break;
        }

        public override void OnPointerDown()
        {
            _clickTween?.Kill();
            _clickTween = _rectTransform.DOAnchorPosY(_originAnchoredPosition.y - yOffset, translationDuration).SetEase(Ease.OutQuad);
        }

        public override void OnPointerUp()
        {
            _clickTween?.Kill();
            _clickTween = _rectTransform.DOAnchorPosY(_originAnchoredPosition.y, translationDuration).SetEase(Ease.OutQuad);
        }
    }
}