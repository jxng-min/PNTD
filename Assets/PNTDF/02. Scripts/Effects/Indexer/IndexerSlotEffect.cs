using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Indexer", "Indexer Slot", 0)]
    public class IndexerSlotEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("Pointer Enter/Exit")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private float hoverDuration = 0.1f;
         
        [Header("Pointer Down/Up")]
        [SerializeField] private float xOffset = 10f;
        [SerializeField] private float translateDuration = 0.2f;
        
        public Tween PlayPointerEnterEffect(Image indexerImage)
        {
            SoundManager.Instance.PlaySFX("SFX_Hover1");
            return indexerImage.DOColor(hoverColor, hoverDuration);
        }
        
        public Tween PlayPointerExitEffect(Image indexerImage)
        {
            return indexerImage.DOColor(defaultColor, hoverDuration);
        }

        public Tween PlayPointerDownEffect(RectTransform rectTransform, Vector2 originAnchoredPosition)
        {
            SoundManager.Instance.PlaySFX("SFX_Click");
            return rectTransform.DOAnchorPosX(originAnchoredPosition.x + xOffset, translateDuration);
        }

        public Tween PlayPointerUpEffect(RectTransform rectTransform, Vector2 originAnchoredPosition)
        {
            return rectTransform.DOAnchorPosX(originAnchoredPosition.x, translateDuration);
        }
    }
}