using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ClearFlowView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private ShadowableLabel clearLabel;

        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private ClearFlowEffect clearFlowEffect;
        
        public IEnumerator ClearRoutine()
        {
            yield return clearFlowEffect.PlayClearFlowEffect(CanvasGroup, clearLabel);
        }
    }
}