using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class GameClearView : ViewBase
    {
        [SerializeField] private JxButton snkrxButton;
        [SerializeField] private JxButton itchButton;
        [SerializeField] private JxButton loopButton;

        private void Awake()
        {
            snkrxButton.AddListener(HandleOnClickedSNKRX);
            itchButton.AddListener(HandleOnClickedITCH);
            loopButton.AddListener(HandleOnClickedLoop);
        }

        public IEnumerator GameClearRoutine()
        {
            CanvasGroup.Show();
            yield return null;
        }

        private void HandleOnClickedSNKRX()
        {
            Application.OpenURL("https://store.steampowered.com/app/915310/SNKRX/");
        }

        private void HandleOnClickedITCH()
        {
            Application.OpenURL("https://jxngmin.itch.io/");
        }

        private IEnumerator HandleOnClickedLoop()
        {
            CanvasGroup.Hide();
            yield return GameFlow.Instance.ContinueLoopRoutine();
        }

        private void OnDestroy()
        {
            snkrxButton.RemoveAllListeners();
            itchButton.RemoveAllListeners();
            loopButton.RemoveAllListeners();
        }
    }
}
