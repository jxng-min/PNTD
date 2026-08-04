using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using JxModule;

namespace PNTD
{
    public delegate IEnumerator LoadingBeginAction(string loadingText = null);

    public delegate IEnumerator LoadingAction();
    public delegate IEnumerator LoadingEndAction();
    
    public class LoadingManager : GlobalSingleton<LoadingManager>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public LoadingBeginAction OnLoadingBegin;
        public LoadingAction OnLoading;
        public LoadingEndAction OnLoadingEnd;
        
        private string _targetSceneName;

        public IEnumerator LoadScene(string sceneName, string loadingText = null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _targetSceneName = sceneName;

            yield return StartCoroutine(LoadRoutine(loadingText));
        }

        public IEnumerator VirtualLoadScene(string loadingText, LoadingAction loadingAction = null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            if (OnLoadingBegin != null)
            {
                yield return StartCoroutine(OnLoadingBegin.Invoke(loadingText));
            }
            
            if (loadingAction != null)
            {
                yield return StartCoroutine(loadingAction.Invoke());
            }
            else if (OnLoading != null)
            {
                yield return StartCoroutine(OnLoading.Invoke());
            }
            
            yield return new WaitForSeconds(1f);
            
            if (OnLoadingEnd != null)
            {
                yield return StartCoroutine(OnLoadingEnd.Invoke());
            }
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name != _targetSceneName)
            {
                return;
            }
            
            canvas.worldCamera = Camera.main;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(EndRoutine());
        }
        
        private IEnumerator LoadRoutine(string loadingText = null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (OnLoadingBegin != null)
            {
                yield return StartCoroutine(OnLoadingBegin.Invoke(loadingText));
            }
            
            var op = SceneManager.LoadSceneAsync(_targetSceneName);
            if (op == null)
            {
                yield break;
            }
            
            op.allowSceneActivation = false;
            yield return new WaitForSeconds(1f);
            op.allowSceneActivation = true;
        }

        private IEnumerator EndRoutine()
        {
            if (OnLoadingEnd != null)
            {
                yield return StartCoroutine(OnLoadingEnd.Invoke());
            }
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
