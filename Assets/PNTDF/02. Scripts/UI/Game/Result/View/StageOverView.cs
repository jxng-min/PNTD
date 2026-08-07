using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StageOverView : ViewBase
    {
        [SerializeField] private ShadowableLabel reachedLabel;
        [SerializeField] private JxButton restartButton;

        private void Awake()
        {
            restartButton.AddListener(OnClickedRestart);
        }

        public IEnumerator StageOverRoutine(int reachedStage)
        {
            CanvasGroup.Show();
            reachedLabel.SetText($"reached stage: {reachedStage}");
            yield return null;
        }

        private IEnumerator OnClickedRestart()
        {
            yield return LoadingManager.Instance.LoadScene("Game", "<pop>loading...</pop>");
        }

        private void OnDestroy()
        {
            restartButton.RemoveAllListeners();
        }
    }
}