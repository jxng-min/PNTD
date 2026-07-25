using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Shop", "Shop Slot", 0)]
    public class ShopSlotEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("Pointer Enter/Exit")]
        [SerializeField] private float defaultAlpha = 0f;
        [SerializeField] private float enterAlpha = 1f;
        [SerializeField] private float fadeDuration = 0.3f;
        
        [Header("Pointer Down/Up")]
        [SerializeField] private float anchoredYOffset = 10f;
        [SerializeField] private float translateDuration = 0.1f;
        [SerializeField] private Ease downEase = Ease.OutQuad;
        [SerializeField] private Ease upEase = Ease.OutQuad;

        public Tween PlayPointerEnterEffect(Image hoverImage)
        {
            return hoverImage.DOFade(enterAlpha, fadeDuration);
        }

        public Tween PlayPointerExitEffect(Image hoverImage)
        {
            return hoverImage.DOFade(defaultAlpha, fadeDuration);
        }

        public Tween PlayPointerDownEffect(RectTransform rectTransform, 
                                           Vector2 originAnchoredPosition)
        {
            return rectTransform.DOAnchorPosY(originAnchoredPosition.y - anchoredYOffset, fadeDuration)
                                .SetEase(downEase);
        }

        public Tween PlayPointerUpEffect(RectTransform rectTransform,
                                         Vector2 originAnchoredPosition)
        {
            return rectTransform.DOAnchorPosY(originAnchoredPosition.y, fadeDuration)
                                .SetEase(upEase);
        }
    }
}