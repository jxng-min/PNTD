using System;
using System.Collections.Generic;
using JxModule;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class ShopPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [Header("Top Group")]
        [SerializeField] private TMP_Text goldLabel;
        [SerializeField] private JxButton rerollButton;
        [SerializeField] private JxToggle lockToggle;
        [SerializeField] private ShopLevelView shopLevelView;
        
        [Header("Slot Group")]
        [SerializeField] private ShopSlotView[] shopSlotViews;

        private readonly List<ShopSlotContext> _slotData = new();
        private bool[] _soldOutSlots;
        
        public event Action<HeroDataTableRow, int> OnClickedSlot;
        public event Action OnClickedReroll;
        public event Action<bool> OnChangedLock;
        public event Action OnRequestLevelUp;

        private void Awake()
        {
            _soldOutSlots = new bool[shopSlotViews.Length];
            
            foreach (var shopSlotView in shopSlotViews)
            {
                if (shopSlotView == null)
                {
                    continue;
                }

                shopSlotView.OnClickedSlot += HandleOnClickedSlot;
            }

            shopLevelView.OnRequestLevelUp += HandleOnRequestLevelUp;
            rerollButton.AddListener(HandleOnClickedReroll);
            lockToggle.OnValueChanged += HandleOnChangedLock;
        }

        public void Initialize(ShopSystem shopSystem)
        {
            shopLevelView.Initialize(shopSystem.Level, shopSystem.Exp);
        }
        
        public void SetSoldOut(int slotIndex)
        {
            if (_soldOutSlots == null || slotIndex < 0 ||  slotIndex >= _soldOutSlots.Length)
            {
                return;
            }
            
            _soldOutSlots[slotIndex] = true;
            shopSlotViews[slotIndex].CanvasGroup.Hide();
        }

        public void HandleOnUpdateGold(int gold)
        {
            goldLabel.text = $"<color=#FFD000>${gold}</color>";
        }

        public void HandleOnUpdateLevel(int level, int exp)
        {
            shopLevelView.UpdateLevel(level, exp);
        }

        public void HandleOnRerollShop(IReadOnlyList<ShopSlotContext> contexts)
        {
            _slotData.Clear();
            RefreshSoldOutSlots();

            if (contexts == null)
            {
                HideAllSlots();
                return;
            }
            
            var loopCount = Mathf.Min(contexts.Count, shopSlotViews.Length);
            for (var index = 0; index < loopCount; index++)
            {
                _slotData.Add(contexts[index]);
            }
        }

        private void HandleOnClickedSlot(HeroDataTableRow heroDataTableRow, int slotIndex)
        {
            OnClickedSlot?.Invoke(heroDataTableRow, slotIndex);
        }

        private void HandleOnRequestLevelUp()
        {
            OnRequestLevelUp?.Invoke();
        }

        private void HandleOnClickedReroll()
        {
            OnClickedReroll?.Invoke();
        }

        private void HandleOnChangedLock(bool isOn)
        {
            OnChangedLock?.Invoke(isOn);
        }

        private void RefreshSoldOutSlots()
        {
            if (_soldOutSlots == null || _soldOutSlots.Length != shopSlotViews.Length)
            {
                _soldOutSlots = new bool[shopSlotViews.Length];
                return;
            }

            for (var index = 0; index < _soldOutSlots.Length; index++)
            {
                _soldOutSlots[index] = false;
            }
        }

        private bool IsSoldOut(int slotIndex)
        {
            return _soldOutSlots != null && 
                   slotIndex >= 0 &&  slotIndex < _soldOutSlots.Length &&
                   _soldOutSlots[slotIndex];
        }

        private void HideAllSlots()
        {
            foreach (var shopSlotView in shopSlotViews)
            {
                if (shopSlotView == null)
                {
                    continue;
                }

                shopSlotView.CanvasGroup.Hide();
            }
        }

        private void OnDestroy()
        {
            foreach (var shopSlotView in shopSlotViews)
            {
                if (shopSlotView == null)
                {
                    continue;
                }
                
                shopSlotView.OnClickedSlot -= HandleOnClickedSlot;
            }
        }
    }
}

