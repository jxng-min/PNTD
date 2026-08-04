using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField] private StageClearView stageClearView;
        [SerializeField] private StageOverView stageOverView;

        public IEnumerator StageClear(int gold, int bonus, int interest)
        {
            yield return stageClearView.StageClearRoutine(gold, bonus, interest);
        }

        public IEnumerator StageOver(int reachedStage)
        {
            yield return stageOverView.StageOverRoutine(reachedStage);
        }
    }
}