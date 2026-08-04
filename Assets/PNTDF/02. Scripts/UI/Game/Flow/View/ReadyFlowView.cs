using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ReadyFlowView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private ShadowableLabel readyLabel;
        [SerializeField] private ShadowableLabel countLabel;

        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private ReadyFlowEffect readyFlowEffect;
        
        public IEnumerator ReadyRoutine(int delay)
        {
            yield return readyFlowEffect.PlayReadyFlowEffect(delay, CanvasGroup, readyLabel, countLabel);
        }
    }
}