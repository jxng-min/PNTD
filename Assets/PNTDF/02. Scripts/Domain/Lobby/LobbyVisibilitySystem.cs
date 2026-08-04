using JxModule;
using UnityEngine;

namespace PNTD
{
    public class LobbyVisibilitySystem
    {
        private readonly CanvasGroup[] _canvasGroups;

        public LobbyVisibilitySystem(CanvasGroup[] canvasGroups)
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
