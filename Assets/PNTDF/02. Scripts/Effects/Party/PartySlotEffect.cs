using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Party", "Party Slot", 0)]
    public class PartySlotEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("Pointer Enter/Exit")]
        [SerializeField] private float punchAmount = 0.05f;
        [SerializeField] private float punchDuration = 0.2f;
        [SerializeField] private float defaultAlpha = 0f;
        [SerializeField] private float hoverAlpha = 1f;
        [SerializeField] private float fadeDuration = 0.3f;
        
        [Header("Pointer Down/Up")]
        [SerializeField] private float yOffset = 5f;
        [SerializeField] private float translateDuration = 0.2f;
        
        [Header("Sorting")]
        [SerializeField] private float sortingDuration = 0.2f;
        [SerializeField] private Ease sortingEase = Ease.OutCubic;

        public Tween PlayPointerEnterEffect(RectTransform rectTransform, Image hoverImage)
        {
            SoundManager.Instance.PlaySFX("SFX_Hover2");
            var sequence = DOTween.Sequence();

            sequence.Append(
                rectTransform.DOPunchScale(new Vector3(punchAmount, punchAmount, 0f), punchDuration)
            );

            sequence.Join(
                hoverImage.DOFade(hoverAlpha, fadeDuration)
            );

            return sequence;
        }

        public Tween PlayPointerExitEffect(RectTransform rectTransform, Vector3 originScale, Image hoverImage)
        {
            rectTransform.localScale = originScale;
            return hoverImage.DOFade(defaultAlpha, fadeDuration);
        }

        public Tween PlayPointerDownEffect(RectTransform rectTransform, Vector2 originAnchoredPosition)
        {
            SoundManager.Instance.PlaySFX("SFX_Click");
            return rectTransform.DOAnchorPosY(originAnchoredPosition.y - yOffset, translateDuration);
        }

        public Tween PlayPointerUpEffect(RectTransform rectTransform, Vector2 originAnchoredPosition)
        {
            return rectTransform.DOAnchorPosY(originAnchoredPosition.y, translateDuration);
        }

        public Tween PlaySortingEffect(RectTransform rectTransform, Vector2 originAnchoredPosition)
        {
            return rectTransform.DOAnchorPos(originAnchoredPosition, sortingDuration).SetEase(sortingEase);
        }
    }
}