using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StageVisibilitySystem
    {
        private readonly CanvasGroup[] _canvasGroups;

        public StageVisibilitySystem(CanvasGroup[] canvasGroups)
        {
            _canvasGroups = canvasGroups;
        }

        public void Show()
        {
            if (_canvasGroups == null)
            {
                return;
            }

            foreach (var canvasGroup in _canvasGroups)
            {
                canvasGroup?.Show();
            }
        }

        public void Hide()
        {
            if (_canvasGroups == null)
            {
                return;
            }

            foreach (var canvasGroup in _canvasGroups)
            {
                canvasGroup?.Hide();
            }
        }
    }
}
