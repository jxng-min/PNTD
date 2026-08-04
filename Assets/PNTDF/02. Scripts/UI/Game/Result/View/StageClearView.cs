using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StageClearView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private ShadowableLabel goldLabel;
        [SerializeField] private ShadowableLabel interestLabel;
        [SerializeField] private ShadowableLabel totalLabel;
        
        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private StageClearEffect stageClearEffect;

        public IEnumerator StageClearRoutine(int gold, int bonus, int interest)
        {
            yield return stageClearEffect.PlayStageClearEffect(gold, bonus, interest, CanvasGroup, goldLabel, interestLabel, totalLabel);
        }
    }
}