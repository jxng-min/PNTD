using System;
using System.Collections.Generic;
using System.Linq;
using JxModule;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PNTD
{
    public class PartyPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [SerializeField] private Canvas partyCanvas;
        [SerializeField] private TMP_Text partyCountLabel;
        [SerializeField] private RectTransform partySlotHolder;
        
        [Space(30f)]
        [BigHeader("Layout")]
        [SerializeField, Min(0f)] private float layoutWidth = 1200f;
        [SerializeField, Min(0f)] private float minimumHorizontalPadding = 100f;
        [SerializeField, Min(0f)] float maximumHorizontalPadding = 300f;
        [SerializeField, Min(0.01f)] private float paddingDecay = 0.5f;
        [SerializeField] private float layoutY;

        private IReadOnlyList<HeroContext> _heroContexts;
        private List<PartySlotView> _partySlotViews;
        private List<PartySlotView> _displayOrders;
        private int _visibleCount;

        private PartySlotView _draggingPartySlot;
        private Vector2 _draggingPointerOffset;
        private int _draggingIndex = -1;
        private bool _isDragging;
        private bool _forceImmediateLayout;

        public event Action OnUpdateCountRequested;
        public event Action OnRefreshSlotsRequested;
        public event Action<HeroContext, int> OnClickedPartySlot;
        public event Action<List<HeroContext>> OnReorderPartyRequested;

        private void Awake()
        {
            _partySlotViews = partySlotHolder.GetComponentsInChildren<PartySlotView>(true).ToList();
            _displayOrders = new List<PartySlotView>(_partySlotViews);

            foreach (var partySlotView in _partySlotViews)
            {
                if (partySlotView == null)
                {
                    continue;
                }
                
                partySlotView.OnSlotClicked += HandleOnSlotClicked;
                partySlotView.OnSlotBeginDrag += HandleOnSlotBeginDrag;
                partySlotView.OnSlotDrag += HandleOnSlotDrag;
                partySlotView.OnSlotEndDrag += HandleOnSlotEndDrag;
            }
        }

        private void Start()
        {
            RefreshPartySlots();
        }

        public void UpdateCountLabel(int currentCount, int maxCount)
        {
            if (partyCountLabel == null)
            {
                return;
            }
            
            partyCountLabel.text = $"{currentCount}/{maxCount}";
        }

        public void UpdatePartySlots(IReadOnlyList<HeroContext> heroContexts, int heroCount, int visibleCount)
        {
            _heroContexts = heroContexts;
            _visibleCount = visibleCount;

            for (var index = 0; index < _displayOrders.Count; index++)
            {
                var partySlotView = _displayOrders[index];
                if (partySlotView == null)
                {
                    continue;
                }
                
                var isActive = index < visibleCount;
                partySlotView.gameObject.SetActive(isActive);

                if (!isActive)
                {
                    partySlotView.ClearPartySlot();
                    continue;
                }

                var slotPosition = GetPositionByIndex(index);
                partySlotView.SetOriginPosition(slotPosition);

                if (_forceImmediateLayout)
                {
                    partySlotView.RectTransform.anchoredPosition = slotPosition;
                }
                else
                {
                    partySlotView.TweenOriginPosition(slotPosition);
                }

                if (index < heroCount)
                {
                    partySlotView.Initialize(heroContexts[index]);
                }
                else
                {
                    partySlotView.ClearPartySlot();
                }
            }

            _forceImmediateLayout = false;
        }
        
#region EventHandling
        public void HandleHeroAdded(HeroContext heroContext)
        {
            _forceImmediateLayout = true;
            RefreshPartySlots();
        }

        public void HandlePartyChanged(HeroContext heroContext)
        {
            RefreshPartySlots();
        }
        
        public void HandlePartyChanged()
        {
            RefreshPartySlots();
        }

        public void HandleHeroCountLimitChanged(int count)
        {
            RefreshPartySlots();
        }

        private void HandleOnSlotClicked(HeroContext heroContext, int price)
        {
            OnClickedPartySlot?.Invoke(heroContext, price);
        }

        private void HandleOnSlotBeginDrag(PartySlotView partySlotView, PointerEventData eventData)
        {
            if (partySlotView == null || partySlotView.HeroContext == null)
            {
                return;
            }

            if (_displayOrders == null || _displayOrders.Count <= 0)
            {
                SyncDisplayOrderToHeroes();
            }

            if (_displayOrders == null || !_displayOrders.Contains(partySlotView))
            {
                return;
            }

            if (!TryGetLocalPosition(eventData.position, out var localPoint))
            {
                return;
            }

            _isDragging = true;
            _draggingPartySlot = partySlotView;
            _draggingIndex = _displayOrders.IndexOf(partySlotView);
            _draggingPointerOffset = partySlotView.RectTransform.anchoredPosition - localPoint;
            
            partySlotView.transform.SetAsLastSibling();
        }

        private void HandleOnSlotDrag(PartySlotView partySlotView, PointerEventData eventData)
        {
            if (!_isDragging || 
                _draggingPartySlot == null || 
                partySlotView != _draggingPartySlot ||
                _partySlotViews == null)
            {
                return;
            }

            if (!TryGetLocalPosition(eventData.position, out var localPoint))
            {
                return;
            }

            if (_visibleCount <= 0)
            {
                return;
            }

            var desiredPosition = localPoint + _draggingPointerOffset;
            desiredPosition.y = layoutY;

            var firstX = GetPositionByIndex(0).x;
            var lastX = GetPositionByIndex(_visibleCount - 1).x;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, Mathf.Min(firstX, lastX), Mathf.Max(firstX, lastX));

            var targetIndex = GetTargetIndex(desiredPosition.x, _visibleCount);
            if (targetIndex >= 0 && targetIndex != _draggingIndex)
            {
                RebuildDisplayOrder(targetIndex);
                _draggingIndex = targetIndex;
            }

            ApplyPartyLayout(desiredPosition);
        }

        private void HandleOnSlotEndDrag(PartySlotView partySlotView, PointerEventData eventData)
        {
            if (!_isDragging || _draggingPartySlot == null || partySlotView != _draggingPartySlot)
            {
                return;
            }
            
            _isDragging = false;
            _forceImmediateLayout = true;

            var orderedParty = _displayOrders
                .Where(view => view != null && view.HeroContext != null)
                .Select(view => view.HeroContext)
                .ToList();
            
            OnReorderPartyRequested?.Invoke(orderedParty);
            
            RestoreSiblingOrder();

            _draggingPartySlot = null;
            _draggingIndex = -1;
            RefreshPartySlots();
        }
#endregion

        private void RefreshPartySlots()
        {
            OnUpdateCountRequested?.Invoke();

            if (_isDragging)
            {
                return;
            }

            RequestRefreshPartySlots();
        }

        private void RequestRefreshPartySlots()
        {
            if (_displayOrders == null)
            {
                return;
            }

            OnRefreshSlotsRequested?.Invoke();
        }

        private Vector2 GetPositionByIndex(int index)
        {
            if (_visibleCount <= 0)
            {
                return new Vector2(0f, layoutY);
            }

            index = Mathf.Clamp(index, 0, _visibleCount - 1);

            if (_visibleCount == 1)
            {
                return new Vector2(0f, layoutY);
            }

            var horizontalPadding = GetHorizontalPadding(_visibleCount);
            var usableWidth = Mathf.Max(0f, layoutWidth - horizontalPadding * 2f);

            var left = -usableWidth * 0.5f;
            var spacing = usableWidth / (_visibleCount - 1);
            var x = left + spacing * index;

            return new Vector2(x, layoutY);
        }
        
        private float GetHorizontalPadding(int visibleCount)
        {
            if (visibleCount <= 1)
            {
                return maximumHorizontalPadding;
            }

            var paddingRange = maximumHorizontalPadding - minimumHorizontalPadding;
            var padding = minimumHorizontalPadding + paddingRange * Mathf.Exp(-paddingDecay * (visibleCount - 1));

            return Mathf.Max(minimumHorizontalPadding, padding);
        }

        private int GetTargetIndex(float xPosition, int visibleCount)
        {
            visibleCount = Mathf.Clamp(visibleCount, 0, _displayOrders?.Count ?? 0);

            if (visibleCount <= 0)
            {
                return -1;
            }

            var nearestIndex = 0;
            var nearestDistance = Mathf.Abs(xPosition - GetPositionByIndex(0).x);

            for (var index = 1; index < visibleCount; index++)
            {
                var distance = Mathf.Abs(xPosition - GetPositionByIndex(index).x);
                if (distance >= nearestDistance)
                {
                    continue;
                }

                nearestDistance = distance;
                nearestIndex = index;
            }

            return nearestIndex;
        }

        private void RebuildDisplayOrder(int targetIndex)
        {
            if (_displayOrders == null || _draggingPartySlot == null)
            {
                return;
            }

            var remainingSlots = _displayOrders
                .Where(view => view != null && view != _draggingPartySlot)
                .ToList();

            targetIndex = Mathf.Clamp(targetIndex, 0, remainingSlots.Count);
            remainingSlots.Insert(targetIndex, _draggingPartySlot);
            _displayOrders = remainingSlots;
        }

        private void SyncDisplayOrderToHeroes()
        {
            if (_partySlotViews == null)
            {
                return;
            }

            if (_heroContexts is not { Count: > 0 })
            {
                _displayOrders = new List<PartySlotView>(_partySlotViews);
                return;
            }

            var orderedParty = new List<PartySlotView>(_partySlotViews.Count);
            var remainingSlots = new List<PartySlotView>(_partySlotViews);

            foreach (var heroContext in _heroContexts)
            {
                if (heroContext == null)
                {
                    continue;
                }

                var partySlotView = remainingSlots.FirstOrDefault(
                    view => view != null && view.HeroContext == heroContext);

                if (partySlotView == null)
                {
                    continue;
                }

                orderedParty.Add(partySlotView);
                remainingSlots.Remove(partySlotView);
            }

            orderedParty.AddRange(remainingSlots);
            _displayOrders = orderedParty;
        }

        private void ApplyPartyLayout(Vector2 draggingPosition)
        {
            if (_displayOrders == null)
            {
                return;
            }

            for (var index = 0; index < _displayOrders.Count; index++)
            {
                var partySlotView = _displayOrders[index];
                if (partySlotView == null)
                {
                    continue;
                }

                var isActive = index < _visibleCount;
                partySlotView.gameObject.SetActive(isActive);

                if (!isActive)
                {
                    partySlotView.ClearPartySlot();
                    continue;
                }

                var slotPosition = GetPositionByIndex(index);
                partySlotView.SetOriginPosition(slotPosition);

                if (partySlotView == _draggingPartySlot)
                {
                    partySlotView.RectTransform.anchoredPosition = draggingPosition;
                    continue;
                }

                partySlotView.TweenOriginPosition(slotPosition);
            }
        }
        
        private void RestoreSiblingOrder()
        {
            if (_displayOrders == null)
            {
                return;
            }

            for (var index = 0; index < _displayOrders.Count; index++)
            {
                var partySlotView = _displayOrders[index];
                if (partySlotView != null)
                {
                    partySlotView.transform.SetSiblingIndex(index);
                }
            }
        }

        private bool TryGetLocalPosition(Vector2 dragPosition, out Vector2 localPosition)
        {
            if (partySlotHolder == null)
            {
                localPosition = default;
                return false;
            }
            
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                partySlotHolder, 
                dragPosition, 
                GetEventCamera(), 
                out localPosition
            );
        }

        private Camera GetEventCamera()
        {
            if (partyCanvas == null || partyCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }
            
            return partyCanvas.worldCamera;
        }
        
        private void OnDestroy()
        {
            if (_partySlotViews == null)
            {
                return;
            }
            
            foreach (var partySlotView in _partySlotViews)
            {
                if (partySlotView == null)
                {
                    continue;
                }
                
                partySlotView.OnSlotClicked -= HandleOnSlotClicked;
                partySlotView.OnSlotBeginDrag -= HandleOnSlotBeginDrag;
                partySlotView.OnSlotDrag -= HandleOnSlotDrag;
                partySlotView.OnSlotEndDrag -= HandleOnSlotEndDrag;
            }
        }
    }
}