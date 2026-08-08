using DG.Tweening;
using JxModule;
using UnityEngine;

namespace PNTD
{
    [ManagedEffect("Toast", "Toast", 0)]
    public class ToastEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private float toastDuration = 0.5f;
        [SerializeField] private float toastYOffset = 320f;
        [SerializeField] private Ease toastEase = Ease.OutBack;

        public Tween PlayToastEffect(RectTransform toastRect, Vector2 originAnchoredPosition, float duration)
        {
            var sequence =  DOTween.Sequence();

            sequence.Join(
                toastRect.DOAnchorPosY(originAnchoredPosition.y + toastYOffset, toastDuration)
                         .SetEase(toastEase)
            );

            sequence.AppendInterval(duration);

            sequence.Append(
                toastRect.DOAnchorPosY(originAnchoredPosition.y, toastDuration)
            );

            return sequence;
        }
    }
}