using System.Collections;
using DG.Tweening;
using JxModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Primitive", "Lock Toggle", 1)]
    public class LockToggleClickAnimator : JxPrimitiveAnimator
    {
        [BigHeader("UI")]
        [SerializeField, Required] private Image targetImage;
        [SerializeField, Required] private TMP_Text targetLabel;
        
#region Settings
        [Space(30f)]
        [BigHeader("Settings")]
        [Header("Punch")]
        [SerializeField] private float punchScaleAmount = 0.1f;
        [SerializeField]  private float punchDuration = 0.5f;
        
        [Header("IsOn")]
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private float enabledDuration = 0.2f;
        
        [Header("Translation")]
        [SerializeField] private float yOffset = 10f;
        [SerializeField] private float translationDuration = 0.1f;
#endregion
        private Tween _hoverTween;
        private Tween _toggleTween;
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
            _hoverTween = _rectTransform.DOPunchScale(new Vector3(punchScaleAmount, punchScaleAmount, 0f), punchDuration);
        }

        protected override void OnSelected(bool instant = false)
        {
            _toggleTween?.Kill();
            _toggleTween = targetImage.DOColor(enabledColor, enabledDuration);

            targetLabel.text = $"unlock";
        }

        protected override void OnDeselected(bool instant = false)
        {
            _toggleTween?.Kill();
            _toggleTween = targetImage.DOColor(_originColor, enabledDuration);
            
            targetLabel.text = $"lock";
        }

        public override void OnPointerExit() { }

        public override IEnumerator OnPointerClick() { yield break; }

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