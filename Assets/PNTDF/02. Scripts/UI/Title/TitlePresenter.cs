using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class TitlePresenter : MonoBehaviour
    {
        private static TitlePresenter _activePresenter;

        [BigHeader("UI")]
        [SerializeField] private TitleView titleView;

        private LobbyModel _lobbyModel;
        private bool _isInitialized;
        private bool _isLoading;

        private void Awake()
        {
            titleView ??= GetComponentInChildren<TitleView>(true);
        }

        public void Initialize(LobbyModel lobbyModel)
        {
            _activePresenter = this;
            _lobbyModel = lobbyModel;
            _isInitialized = true;
            _isLoading = false;

            _lobbyModel?.Hide();
            titleView?.Show();
        }

        public static void RequestTitleClick()
        {
            _activePresenter?.HandleTitleClick();
        }

        private void HandleTitleClick()
        {
            if (!_isInitialized || _isLoading)
            {
                return;
            }

            StartCoroutine(EnterLobbyRoutine());
        }

        private IEnumerator EnterLobbyRoutine()
        {
            if (_isLoading)
            {
                yield break;
            }

            _isInitialized = false;
            _isLoading = true;
            yield return LoadingManager.Instance.VirtualLoadScene("<pop>loading...</pop>", ShowLobbyRoutine);
            _isLoading = false;
        }

        private IEnumerator ShowLobbyRoutine()
        {
            if (titleView != null)
            {
                titleView.CanvasGroup.Hide();
            }

            _lobbyModel?.ShowShop(false);
            yield break;
        }

        private void OnDestroy()
        {
            if (_activePresenter == this)
            {
                _activePresenter = null;
            }
        }
    }
}
