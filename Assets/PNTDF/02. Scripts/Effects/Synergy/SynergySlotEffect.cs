using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Synergy", "Synergy Slot", 0)]
    public class SynergySlotEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private float minAlpha = 0.25f;
        [SerializeField] private float alphaDuration = 0.35f;
        
        public Tween PlayHighlightEffect(Image stepImage)
        {
            var sequence = DOTween.Sequence();
            
            sequence.Append(stepImage.DOFade(maxAlpha, alphaDuration));
            sequence.Append(stepImage.DOFade(minAlpha, alphaDuration));
            sequence.SetLoops(-1, LoopType.Yoyo);
            
            return sequence;
        }
    }
}