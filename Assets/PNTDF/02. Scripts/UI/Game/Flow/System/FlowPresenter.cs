using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class FlowPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [SerializeField] private ReadyFlowView readyFlowView;
        [SerializeField] private ClearFlowView clearFlowView;

        public IEnumerator Ready(float delay)
        {
            if (delay <= 0f)
            {
                yield break;
            }
            
            var delay2Int = Mathf.FloorToInt(delay);
            yield return readyFlowView.ReadyRoutine(delay2Int);
        }

        public IEnumerator Clear()
        {
            yield return clearFlowView.ClearRoutine();
        }
    }
}