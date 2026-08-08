using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class IndexerPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [SerializeField] private Transform indexerHolder;
        [SerializeField] private JxButton playButton;
        [SerializeField] private CanvasGroup[] lobbyCanvasGroups;

        private IndexerSlotView[] _indexerSlotViews;

        public event Action OnClickedPlay;

        private void Awake()
        {
            _indexerSlotViews = indexerHolder.GetComponentsInChildren<IndexerSlotView>();

            for (var index = 0; index < _indexerSlotViews.Length; index++)
            {
                var indexerSlotView = _indexerSlotViews[index];
                indexerSlotView.Initialize(index);
                indexerSlotView.OnClickedIndexerSlot += HandleOnClickedIndexerSlot;
            }

            playButton ??= GetComponentsInChildren<JxButton>(true)
                .FirstOrDefault(button => button != null && button.name == "Play Button");
            playButton?.AddListener(HandleOnClickedPlay);
        }

        public void ShowShop()
        {
            HandleEnableCanvasGroup(0);
        }

        private void HandleOnClickedIndexerSlot(int slotIndex)
        {
            
            switch (slotIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    HandleEnableCanvasGroup(slotIndex);
                    break;
                
                case 4:
                    HandleGameRestart();
                    break;
            }
        }

        private void HandleEnableCanvasGroup(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= lobbyCanvasGroups.Length)
            {
                return;
            }
            
            for (var index = 0; index < lobbyCanvasGroups.Length; index++)
            {
                if (index == slotIndex)
                {
                    _indexerSlotViews[index].SetFocus(true);
                    lobbyCanvasGroups[index].DOFade(1f, 0.15f);
                    lobbyCanvasGroups[index].interactable = true;
                    lobbyCanvasGroups[index].blocksRaycasts = true;
                    continue;
                }
                
                _indexerSlotViews[index].SetFocus(false);
                lobbyCanvasGroups[index].alpha = 0f;
                lobbyCanvasGroups[index].interactable = false;
                lobbyCanvasGroups[index].blocksRaycasts = false;
            }
        }

        private void HandleGameRestart()
        {
            StartCoroutine(GameRestartRoutine());
        }
        
        private IEnumerator GameRestartRoutine()
        {
            yield return LoadingManager.Instance.VirtualLoadScene("<pop>loading...</pop>", GameFlow.Instance.ResetGameRoutine);
        }

        private void HandleOnClickedPlay()
        {
            OnClickedPlay?.Invoke();
        }

        private void OnDestroy()
        {
            playButton?.RemoveListener(HandleOnClickedPlay);

            if (_indexerSlotViews == null)
            {
                return;
            }

            foreach (var indexerSlotView in _indexerSlotViews)
            {
                if (indexerSlotView == null)
                {
                    continue;
                }
                
                indexerSlotView.OnClickedIndexerSlot -= HandleOnClickedIndexerSlot;
            }
        }
    }
}
