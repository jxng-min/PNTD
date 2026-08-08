using DG.Tweening;
using JxModule;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class ToastView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private TMP_Text toastLabel;

        [Space(30f)]
        [BigHeader("Effect")] 
        [SerializeField] private ToastEffect toastEffect;

        private Vector2 _originAnchoredPosition;
        private Tween _toastTween;
        
        private void Awake()
        {
            _originAnchoredPosition = RectTransform.anchoredPosition;
        }
        
        public void Show(string toastText, float duration)
        {
            toastLabel.text = toastText;
            _toastTween?.Kill();
            _toastTween = toastEffect.PlayToastEffect(RectTransform, _originAnchoredPosition, duration);
        }
    }
}