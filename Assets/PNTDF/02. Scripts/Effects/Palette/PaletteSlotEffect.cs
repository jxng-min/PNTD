using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Palette", "Palette Slot", 0)]
    public class PaletteSlotEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("Pointer Enter/Exit")]
        [SerializeField] private float punchAmount = 0.1f;
        [SerializeField] private float punchDuration = 0.5f;
        [SerializeField] private float defaultAlpha = 0f;
        [SerializeField] private float hoverAlpha = 1f;
        [SerializeField] private float alphaDuration = 0.1f;
        
        [Header("Pointer Down/Up")]
        [SerializeField] private float clickYOffset = -10f;
        [SerializeField] private float clickDuration = 0.1f;

        public Tween PlayPointerEnterEffect(Image hoverImage)
        {
            var sequence = DOTween.Sequence();

            sequence.Join(
                hoverImage.DOFade(hoverAlpha, alphaDuration)
            );

            sequence.Join(
                hoverImage.transform.DOPunchScale(new Vector3(punchAmount, punchAmount, 0f), punchDuration)
            );
            
            return sequence;
        }

        public Tween PlayPointerExitEffect(Image hoverImage)
        {
            hoverImage.transform.localScale = Vector3.one;
            return hoverImage.DOFade(defaultAlpha, alphaDuration);
        }

        public Tween PlayPointerDownEffect(RectTransform slotRect, Vector2 originAnchoredPosition)
        {
            return slotRect.DOAnchorPosY(originAnchoredPosition.y + clickYOffset, clickDuration);
        }
        
        public Tween PlayPointerUpEffect(RectTransform slotRect, Vector2 originAnchoredPosition)
        {
            return slotRect.DOAnchorPosY(originAnchoredPosition.y, clickDuration);
        }
    }
}