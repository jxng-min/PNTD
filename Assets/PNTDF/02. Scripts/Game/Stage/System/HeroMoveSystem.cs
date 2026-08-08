using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PNTD
{
    public class HeroMoveSystem
    {
        private readonly BoardSystem _boardSystem;
        private readonly DeploySystem _deploySystem;
        private readonly StageMap _stageMap;
        private readonly ClericSanctuarySystem _clericSanctuarySystem;
        private readonly DeployPreviewView _deployPreviewView;
        private readonly HashSet<Hero> _heroes = new();

        private Hero _draggingHero;
        private Vector3Int _originCellPosition;
        private Vector3Int _currentCellPosition;
        private bool _canMove;

        public HeroMoveSystem(BoardSystem boardSystem,
                              DeploySystem deploySystem,
                              StageMap stageMap,
                              ClericSanctuarySystem clericSanctuarySystem,
                              DeployPreviewView deployPreviewView)
        {
            _boardSystem = boardSystem;
            _deploySystem = deploySystem;
            _stageMap = stageMap;
            _clericSanctuarySystem = clericSanctuarySystem;
            _deployPreviewView = deployPreviewView;
        }

        public void Register(Hero hero)
        {
            if (hero?.Dragger == null || !_heroes.Add(hero))
            {
                return;
            }

            hero.Dragger.OnBeginDragRequested += HandleOnBeginDragRequested;
            hero.Dragger.OnDragRequested += HandleOnDragRequested;
            hero.Dragger.OnEndDragRequested += HandleOnEndDragRequested;
        }

        public void Dispose()
        {
            foreach (var hero in _heroes)
            {
                if (hero?.Dragger == null)
                {
                    continue;
                }

                hero.Dragger.OnBeginDragRequested -= HandleOnBeginDragRequested;
                hero.Dragger.OnDragRequested -= HandleOnDragRequested;
                hero.Dragger.OnEndDragRequested -= HandleOnEndDragRequested;
            }

            _heroes.Clear();
            ClearDrag();
        }

        public void Tick()
        {
            if (_draggingHero == null || !Input.GetMouseButtonDown(1))
            {
                return;
            }

            CancelDrag();
        }

        private void HandleOnBeginDragRequested(Hero hero, PointerEventData eventData)
        {
            if (hero == null ||
                eventData.button != PointerEventData.InputButton.Left ||
                _deploySystem is { DeployMode: true } ||
                _stageMap?.BuildMap == null ||
                !_boardSystem.TryGetCell(hero, out _originCellPosition))
            {
                return;
            }

            _draggingHero = hero;
            _boardSystem.ReleaseByHero(hero);
            hero.Attack?.Pause();
            hero.Model?.SetRotationPaused(true);

            _stageMap.MapEffect?.SetHighlight(true);
            _deployPreviewView?.Show(false);

            UpdateDragPosition(eventData.position);
        }

        private void HandleOnDragRequested(Hero hero, PointerEventData eventData)
        {
            if (_draggingHero != hero)
            {
                return;
            }

            UpdateDragPosition(eventData.position);
        }

        private void HandleOnEndDragRequested(Hero hero, PointerEventData eventData)
        {
            if (_draggingHero != hero)
            {
                return;
            }

            UpdateDragPosition(eventData.position);

            if (_canMove)
            {
                MoveOrSwap(hero, _currentCellPosition);
            }
            else
            {
                MoveToCell(hero, _originCellPosition);
                _boardSystem.TryOccupy(_originCellPosition, hero);
                hero.NotifyDeployed(_originCellPosition);
            }
            
            SoundManager.Instance.PlaySFX("SFX_Deploy");

            hero.Attack?.Resume();
            hero.Model?.SetRotationPaused(false);
            ClearDrag();
            _clericSanctuarySystem?.Refresh();
        }

        private void UpdateDragPosition(Vector2 screenPosition)
        {
            if (_draggingHero == null || _stageMap?.BuildMap == null)
            {
                return;
            }

            var worldPosition = GetWorldPosition(screenPosition);
            _currentCellPosition = _stageMap.BuildMap.WorldToCell(worldPosition);
            var cellCenter = _stageMap.BuildMap.GetCellCenterWorld(_currentCellPosition);
            _draggingHero.transform.position = cellCenter;
            _deployPreviewView?.SetWorldPosition(cellCenter);

            _canMove = CanMove(_currentCellPosition);
        }

        private bool CanMove(Vector3Int cellPosition)
        {
            return _stageMap.BuildMap.HasTile(cellPosition);
        }

        private void MoveOrSwap(Hero hero, Vector3Int targetCellPosition)
        {
            if (hero == null)
            {
                return;
            }

            if (_boardSystem.TryGetHero(targetCellPosition, out var targetHero) && targetHero != null && targetHero != hero)
            {
                _boardSystem.ReleaseByHero(targetHero);

                MoveToCell(targetHero, _originCellPosition);
                _boardSystem.TryOccupy(_originCellPosition, targetHero);
                targetHero.NotifyDeployed(_originCellPosition);
            }

            MoveToCell(hero, targetCellPosition);
            _boardSystem.TryOccupy(targetCellPosition, hero);
            hero.NotifyDeployed(targetCellPosition);
        }

        private void MoveToCell(Hero hero, Vector3Int cellPosition)
        {
            if (hero == null || _stageMap?.BuildMap == null)
            {
                return;
            }

            hero.transform.position = _stageMap.BuildMap.GetCellCenterWorld(cellPosition);
        }

        private static Vector3 GetWorldPosition(Vector2 screenPosition)
        {
            var targetCamera = Camera.main;
            if (targetCamera == null)
            {
                return Vector3.zero;
            }

            var position = new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs(targetCamera.transform.position.z));
            return targetCamera.ScreenToWorldPoint(position);
        }

        private void ClearDrag()
        {
            _draggingHero?.Attack?.Resume();
            _draggingHero?.Model?.SetRotationPaused(false);
            _stageMap?.MapEffect?.SetHighlight(_deploySystem is { DeployMode: true });
            _deployPreviewView?.Hide();

            _draggingHero = null;
            _originCellPosition = default;
            _currentCellPosition = default;
            _canMove = false;
        }

        private void CancelDrag()
        {
            var hero = _draggingHero;
            if (hero == null)
            {
                return;
            }

            MoveToCell(hero, _originCellPosition);
            _boardSystem.TryOccupy(_originCellPosition, hero);
            hero.NotifyDeployed(_originCellPosition);
            ClearDrag();
            _clericSanctuarySystem?.Refresh();
        }
    }
}
